namespace Auth.Application.Abstractions.Authentication;

public interface ICurrentUser
{
    public Guid UserId { get; }
}