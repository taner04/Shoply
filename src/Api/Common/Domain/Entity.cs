namespace Api.Common.Domain;

public abstract class Entity<TId> : Auditable where TId : struct
{
    public TId Id { get; protected init; }
}