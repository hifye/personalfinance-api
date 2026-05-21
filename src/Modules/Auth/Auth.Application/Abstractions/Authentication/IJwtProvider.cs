using Auth.Application.Features.Authentication.Responses;
using Auth.Domain.Entities;

namespace Auth.Application.Abstractions.Authentication;

public interface IJwtProvider
{
    TokenResponse Generate(User user);
}