using ScienceAtrium.Domain.Entities;
using ScienceAtrium.Domain.Entities.Interfaces;

namespace ScienceAtrium.Domain.Entities.Interfaces;

public interface IRepositoryAsync<TEntity> : IBase<TEntity>, IReaderAsync<TEntity>, IWriterAsync<TEntity> where TEntity : Entity
{
}
