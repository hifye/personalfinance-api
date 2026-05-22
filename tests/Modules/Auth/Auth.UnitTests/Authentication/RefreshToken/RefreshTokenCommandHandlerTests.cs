using Auth.Application.Abstractions.Authentication;
using Auth.Application.Abstractions.Persistance;
using Auth.Application.Abstractions.Security;
using Auth.Application.Features.Authentication.RefreshToken;
using Auth.Application.Features.Authentication.Responses;
using Auth.Domain.Entities;
using BuildingBlocks.Application.Abstractions;
using FluentAssertions;
using NSubstitute;
using SharedKernel.Common;
using Xunit;

namespace Auth.UnitTests.Authentication.RefreshToken;

public sealed class RefreshTokenCommandHandlerTests
{
    private readonly IRefreshTokenRepository _refreshTokenRepositoryMock;
    private readonly IUserRepository _userRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IJwtProvider _jwtProviderMock;
    private readonly IRefreshTokenHasher _refreshTokenHasherMock;
    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _refreshTokenRepositoryMock = Substitute.For<IRefreshTokenRepository>();
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _jwtProviderMock = Substitute.For<IJwtProvider>();
        _refreshTokenHasherMock = Substitute.For<IRefreshTokenHasher>();

        _handler = new RefreshTokenCommandHandler(
            _refreshTokenRepositoryMock,
            _userRepositoryMock,
            _unitOfWorkMock,
            _jwtProviderMock,
            _refreshTokenHasherMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenTokenDoesNotExist()
    {
        // Arrange
        var command = new RefreshTokenCommand("invalid_token");
        _refreshTokenHasherMock.Hash(command.RefreshToken).Returns("token_hash");
        _refreshTokenRepositoryMock.GetRefreshTokenByHash("token_hash", Arg.Any<CancellationToken>()).Returns((Auth.Domain.Entities.RefreshToken?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Invalid or expired refresh token.");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserNotFound()
    {
        // Arrange
        var command = new RefreshTokenCommand("valid_token");
        var refreshToken = Auth.Domain.Entities.RefreshToken.Create(Guid.NewGuid(), "token_hash", DateTime.UtcNow.AddDays(1)).Value;
        
        _refreshTokenHasherMock.Hash(command.RefreshToken).Returns("token_hash");
        _refreshTokenRepositoryMock.GetRefreshTokenByHash("token_hash", Arg.Any<CancellationToken>()).Returns(refreshToken);
        _userRepositoryMock.GetUserById(refreshToken!.UserId).Returns((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("User not found.");
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenTokenIsValid()
    {
        // Arrange
        var command = new RefreshTokenCommand("valid_token");
        var userId = Guid.NewGuid();
        var refreshToken = Auth.Domain.Entities.RefreshToken.Create(userId, "token_hash", DateTime.UtcNow.AddDays(1)).Value;
        var user = User.Create("Test", "test@example.com", "hash").Value;
        var tokenResponse = new TokenResponse("new_access_token", "new_refresh_token", DateTime.UtcNow.AddDays(1));

        _refreshTokenHasherMock.Hash(command.RefreshToken).Returns("token_hash");
        _refreshTokenRepositoryMock.GetRefreshTokenByHash("token_hash", Arg.Any<CancellationToken>()).Returns(refreshToken);
        _userRepositoryMock.GetUserById(userId).Returns(user);
        _jwtProviderMock.Generate(user!).Returns(tokenResponse);
        _refreshTokenHasherMock.Hash(tokenResponse.RefreshToken).Returns("new_token_hash");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(tokenResponse);
        await _refreshTokenRepositoryMock.Received(1).RevokeRefreshToken(refreshToken!.Id, Arg.Any<CancellationToken>());
        await _refreshTokenRepositoryMock.Received(1).CreateRefreshToken(Arg.Any<Auth.Domain.Entities.RefreshToken>(), Arg.Any<CancellationToken>());
        await _unitOfWorkMock.Received(1).CommitAsync();
    }
}

