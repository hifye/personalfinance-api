namespace Auth.Application.Authentication.Responses;

public record TokenResponse( 
    string Token,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt);