using ScienceAtrium.Domain.Entities;

namespace ScienceAtrium.Domain.Entities.Interfaces;

public interface IWriterAsync<in TEntity> where TEntity : Entity
{
    /// <summary>
    /// adds entity to database
    /// </summary>
    /// <param name="entity">inherited model type</param>
    /// <returns></returns>
    Task<int> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// updates entity in database
    /// </summary>
    /// <param name="entity">inherited model type</param>
    /// <returns></returns>
    Task<int> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// deletes entity in database
    /// </summary>
    /// <param name="entity">inherited model type</param>
    /// <returns></returns>
    Task<int> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
}
