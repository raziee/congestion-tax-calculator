namespace CongestionCalculator.Domain.SeedWork;

public abstract class EntityBase<TId>
    where TId : notnull
{
    public TId? Id { get; protected set; }

    protected EntityBase(TId id) => Id = id;

    protected EntityBase() { }

    public override bool Equals(object? obj)
    {
        if (obj is not EntityBase<TId> other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (Id == null || other.Id == null || Id.Equals(default) || other.Id.Equals(default))
            return false;

        return Id.Equals(other.Id);
    }

    public override int GetHashCode() => (GetType().ToString() + Id).GetHashCode();
}
