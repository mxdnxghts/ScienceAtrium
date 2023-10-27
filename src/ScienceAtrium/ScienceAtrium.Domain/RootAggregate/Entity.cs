using ScienceAtrium.Domain.RootAggregate.Interfaces;
using System.Linq.Expressions;

namespace ScienceAtrium.Domain.RootAggregate;

public abstract class Entity : IEquatable<Entity>, IEntityValidation<Entity>
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

    public bool IsExist(IReader<Entity> reader, Expression<Func<Entity, bool>> func)
    {
        return reader.Exist(func);
    }

    public bool IsEmpty(Guid id)
    {
        return id == Guid.Empty;
    }

    public bool IsNull(Entity entity)
    {
        return entity is null;
    }

    public bool IsValid(IReader<Entity> reader, Entity entity)
    {
        return IsValid(reader, x => x.Id == entity.Id, entity);
    }

    public bool IsValid(IReader<Entity> reader, Expression<Func<Entity, bool>> func, Entity entity)
    {
        return !IsNull(entity) && !IsEmpty(entity.Id) && IsExist(reader, func);
    }
}
