using Auth.Application.Abstractions.Persistance;
using Auth.Application.Features.Authentication.Logout;
using BuildingBlocks.Application.Abstractions;
using FluentAssertions;
using NSubstitute;

namespace Auth.UnitTests.Authentication.Logout;

public sealed class LogoutCommandHandlerTests
{
    private readonly IRefreshTokenRepository _refreshTokenRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ICurrentUser _currentUserMock;
    private readonly LogoutCommandHandler _handler;

    public LogoutCommandHandlerTests()
    {
        _refreshTokenRepositoryMock = Substitute.For<IRefreshTokenRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _currentUserMock = Substitute.For<ICurrentUser>();

        _handler = new LogoutCommandHandler(
            _refreshTokenRepositoryMock,
            _unitOfWorkMock,
            _currentUserMock);
    }

    [Fact]
    public async Task Handle_ShouldRevokeAllTokensAndCommit()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserMock.UserId.Returns(userId);
        var command = new LogoutCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _refreshTokenRepositoryMock.Received(1).RevokeAllUserTokens(userId, Arg.Any<CancellationToken>());
        await _unitOfWorkMock.Received(1).CommitAsync();
    }
}

