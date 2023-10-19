using ScienceAtrium.Domain.Entities;
using ScienceAtrium.Domain.Entities.Interfaces;

namespace ScienceAtrium.Domain.Entities.Interfaces;

public interface IRepository<TEntity> : IBase<TEntity>, IReader<TEntity>, IWriter<TEntity>, IDisposable where TEntity : Entity
{
}
