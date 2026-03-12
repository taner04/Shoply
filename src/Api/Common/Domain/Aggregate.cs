namespace Api.Common.Domain;

public abstract class Aggregate<TId> : Entity<TId> where TId : struct;