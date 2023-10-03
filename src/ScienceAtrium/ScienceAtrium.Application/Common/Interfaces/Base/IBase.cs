using ScienceAtrium.Domain.Entities;

namespace ScienceAtrium.Application.Common.Interfaces.Base;
public interface IBase<out TEntity> where TEntity : Entity
{
    /// <summary>
    /// returns collection of entities
    /// </summary>
    IQueryable<TEntity> All { get; }
}
