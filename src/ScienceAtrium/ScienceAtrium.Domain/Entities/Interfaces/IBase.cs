namespace ScienceAtrium.Domain.Entities.Interfaces;
public interface IBase<out TEntity> where TEntity : Entity
{
    /// <summary>
    /// returns collection of entities
    /// </summary>
    IQueryable<TEntity> All { get; }
}
