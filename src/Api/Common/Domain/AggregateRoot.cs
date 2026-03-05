namespace Api.Common.Domain;

public abstract class AggregateRoot<TId> : Entity<TId> where TId : struct;