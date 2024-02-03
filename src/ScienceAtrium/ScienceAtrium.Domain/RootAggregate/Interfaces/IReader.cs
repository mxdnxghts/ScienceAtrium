using System.Linq.Expressions;

namespace ScienceAtrium.Domain.RootAggregate.Interfaces;

public interface IReader<TEntity> where TEntity : Entity
{
	/// <summary>
	/// returns entity with specified predicate from database
	/// if you'll pass only a predicate, it will return an entity from the database
    /// and 'll skip check in cache
	/// </summary>
	/// <param name="id">entity id</param>
	/// <param name="predicate">predicate for function expression</param>
	/// <returns></returns>
	TEntity Get(Guid? id = null, Expression<Func<TEntity, bool>>? predicate = null);

    /// <summary>
    /// returns true or false depending exists entity in the database or not
    /// </summary>
    /// <param name="id">entity id</param>
    /// <param name="predicate">predicate for function expression</param>
    /// <returns></returns>
    bool Exist(Guid? id = null, Expression<Func<TEntity, bool>>? predicate = null);

    /// <summary>
    /// checks if the passed entity meets the conditions
    /// </summary>
    /// <param name="entity">inherited model type</param>
    /// <returns></returns>
    bool FitsConditions(TEntity? entity);
}
