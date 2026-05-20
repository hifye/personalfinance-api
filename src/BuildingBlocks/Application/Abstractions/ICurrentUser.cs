namespace BuildingBlocks.Application.Abstractions;

public interface ICurrentUser
{
    public Guid UserId { get; }
}