using ScienceAtrium.Domain.Entities;
using System.Linq.Expressions;

namespace ScienceAtrium.Application.Common.Interfaces.Base;

public interface IReaderAsync<TEntity> where TEntity : Entity
{
    /// <summary>
    /// returns entity with specified predicate from database
    /// </summary>
    /// <param name="predicate">predicate for function expression</param>
    /// <returns></returns>
    Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// returns true or false depending exists entity in the database or not
    /// </summary>
    /// <param name="predicate">predicate for function expression</param>
    /// <returns></returns>
    Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// checks if the passed item meets the conditions
    /// </summary>
    /// <param name="item">inherited model type</param>
    /// <returns></returns>
    Task<bool> FitsConditionsAsync(TEntity? item);
}
