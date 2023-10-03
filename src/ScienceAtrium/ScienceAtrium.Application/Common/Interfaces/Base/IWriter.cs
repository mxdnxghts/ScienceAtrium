using ScienceAtrium.Domain.Entities;

namespace ScienceAtrium.Application.Common.Interfaces.Base;

public interface IWriter<in TEntity> where TEntity : Entity
{
    /// <summary>
    /// adds entity to database
    /// </summary>
    /// <param name="item">inherited model type</param>
    /// <returns></returns>
    int Create(TEntity item);

    /// <summary>
    /// updates entity in database
    /// </summary>
    /// <param name="item">inherited model type</param>
    /// <returns></returns>
    int Update(TEntity item);

    /// <summary>
    /// deletes entity in database
    /// </summary>
    /// <param name="item">inherited model type</param>
    /// <returns></returns>
    int Delete(TEntity item);
}
