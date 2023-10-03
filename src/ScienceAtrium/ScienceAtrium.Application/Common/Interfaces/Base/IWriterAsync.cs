using ScienceAtrium.Domain.Entities;

namespace ScienceAtrium.Application.Common.Interfaces.Base;

public interface IWriterAsync<in TEntity> where TEntity : Entity
{
    /// <summary>
    /// adds entity to database
    /// </summary>
    /// <param name="item">inherited model type</param>
    /// <returns></returns>
    Task<int> CreateAsync(TEntity item);

    /// <summary>
    /// updates entity in database
    /// </summary>
    /// <param name="item">inherited model type</param>
    /// <returns></returns>
    Task<int> UpdateAsync(TEntity item);

    /// <summary>
    /// deletes entity in database
    /// </summary>
    /// <param name="item">inherited model type</param>
    /// <returns></returns>
    Task<int> DeleteAsync(TEntity item);
}