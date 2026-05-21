namespace Auth.Application.Features.Authentication.Responses;

public record TokenResponse( 
    string AccessToken,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt);