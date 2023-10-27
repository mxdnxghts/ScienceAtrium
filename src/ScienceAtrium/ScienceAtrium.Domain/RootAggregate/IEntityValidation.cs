using ScienceAtrium.Domain.RootAggregate.Interfaces;
using System.Linq.Expressions;

namespace ScienceAtrium.Domain.RootAggregate;
public interface IEntityValidation<TEntity> where TEntity : Entity
{
    /// <summary>
    /// check  entity on null
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    bool IsNull(TEntity entity);

    /// <summary>
    /// check entity on default value
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    bool IsEmpty(Guid id);

    /// <summary>
    /// check entity on existing in the database
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    bool IsExist(IReader<TEntity> reader, Expression<Func<TEntity, bool>> func);

    /// <summary>
    /// combine all checks in one
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    bool IsValid(IReader<TEntity> reader,  TEntity entity);

    /// <summary>
    /// combine all checks in one
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="func"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    bool IsValid(IReader<TEntity> reader, Expression<Func<TEntity, bool>> func, TEntity entity);
}
