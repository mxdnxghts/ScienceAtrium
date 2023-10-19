using ScienceAtrium.Domain.Entities;

namespace ScienceAtrium.Domain.Entities.Interfaces;

public interface IWriter<in TEntity> where TEntity : Entity
{
    /// <summary>
    /// adds entity to database
    /// </summary>
    /// <param name="entity">inherited model type</param>
    /// <returns></returns>
    int Create(TEntity entity);

    /// <summary>
    /// updates entity in database
    /// </summary>
    /// <param name="entity">inherited model type</param>
    /// <returns></returns>
    int Update(TEntity entity);

    /// <summary>
    /// deletes entity in database
    /// </summary>
    /// <param name="entity">inherited model type</param>
    /// <returns></returns>
    int Delete(TEntity entity);
}
