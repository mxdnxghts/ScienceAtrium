using System.Linq.Expressions;

namespace ScienceAtrium.Domain.RootAggregate.Options;
public sealed class EntityFindOptions<TEntity>(Guid? entityId = null, Expression<Func<TEntity, bool>>? predicate = null)
    where TEntity : Entity
{
    private bool _onlyDatabaseFind;
	public Guid? EntityId { get; init; } = entityId;
	public Expression<Func<TEntity, bool>>? Predicate { get; init; } = predicate;
	public bool OnlyDatabaseFind => _onlyDatabaseFind;

    public EntityFindOptions<TEntity> ForceDatabaseSearch()
    {
        if (Predicate is not null || IsValidEntityId())
            _onlyDatabaseFind = true;

        return this;
    }

    private bool IsValidEntityId()
    {
        return EntityId != null && EntityId != Guid.Empty;
	}
}
