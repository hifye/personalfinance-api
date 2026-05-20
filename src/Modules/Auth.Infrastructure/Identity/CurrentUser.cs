using System.Security.Claims;
using Auth.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;

namespace Auth.Infrastructure.Identity;

public class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public Guid UserId => Guid.Parse(accessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                                     throw new Exception("User ID claim not found"));
}