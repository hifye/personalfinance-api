using System;

namespace Auth.Application.Authentication.Responses;

public record TokenResponse( 
    string AccessToken,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt);