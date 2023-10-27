using ScienceAtrium.Domain.RootAggregate.Interfaces;
using System.Linq.Expressions;

namespace ScienceAtrium.Domain.RootAggregate;

public abstract class Entity : IEquatable<Entity>, IEntityValidation
{
    protected Entity(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; private init; }

    public bool Equals(Entity? other)
    {
        if (other is null)
            return false;

        if (other.GetType() != GetType())
            return false;

        return other.Id == Id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (obj.GetType() != GetType())
            return false;

        if (obj is not Entity entity)
            return false;

        return entity.Id == Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode() * 12;
    }

    public bool IsEmpty()
    {
        return Id == Guid.Empty;
    }

    public bool IsExist<TReaderEntity>(IReader<TReaderEntity> reader, Expression<Func<TReaderEntity, bool>> func)
        where TReaderEntity : Entity
    {
        return reader.Exist(func);
    }

    public bool IsValid<TReaderEntity>(IReader<TReaderEntity> reader)
        where TReaderEntity : Entity
    {
        return IsValid(reader, x => x.Id == Id);
    }

    public bool IsValid<TReaderEntity>(IReader<TReaderEntity> reader, Expression<Func<TReaderEntity, bool>> func)
        where TReaderEntity : Entity
    {
        return !IsEmpty() && IsExist(reader, func);
    }
}
