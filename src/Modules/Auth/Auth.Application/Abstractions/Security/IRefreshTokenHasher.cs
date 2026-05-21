namespace Auth.Application.Abstractions.Security;

public interface IRefreshTokenHasher
{
    string Hash(string token);
}