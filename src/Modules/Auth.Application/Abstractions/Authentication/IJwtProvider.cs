using Auth.Application.Authentication.Responses;
using Auth.Domain.Entities;

namespace Auth.Application.Abstractions.Authentication;

public interface IJwtProvider
{
    TokenResponse Generate(User user);
}