namespace Auth.Application.Authentication.Responses;

public record TokenResponse( 
    string AccesToken,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt);