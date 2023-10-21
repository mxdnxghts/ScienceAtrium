using System.Linq.Expressions;
using ScienceAtrium.Domain.RootAggregate;

namespace ScienceAtrium.Domain.RootAggregate.Interfaces;

public interface IReader<TEntity> where TEntity : Entity
{
    /// <summary>
    /// returns entity with specified predicate from database
    /// </summary>
    /// <param name="predicate">predicate for function expression</param>
    /// <returns></returns>
    TEntity Get(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// returns true or false depending exists entity in the database or not
    /// </summary>
    /// <param name="predicate">predicate for function expression</param>
    /// <returns></returns>
    bool Exist(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// checks if the passed entity meets the conditions
    /// </summary>
    /// <param name="entity">inherited model type</param>
    /// <returns></returns>
    bool FitsConditions(TEntity? entity);
}
