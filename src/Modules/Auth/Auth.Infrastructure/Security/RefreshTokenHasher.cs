using System.Security.Cryptography;
using System.Text;
using Auth.Application.Abstractions.Security;

namespace Auth.Infrastructure.Security;

internal sealed class RefreshTokenHasher : IRefreshTokenHasher
{
    public string Hash(string token)
    {
        var bytes =
            SHA256.HashData(
                Encoding.UTF8.GetBytes(token));

        return Convert.ToHexString(bytes);
    }
}