using ScienceAtrium.Domain.RootAggregate;
using ScienceAtrium.Domain.RootAggregate.Interfaces;

namespace ScienceAtrium.Domain.RootAggregate.Interfaces;

public interface IRepositoryAsync<TEntity> : IBase<TEntity>, IReaderAsync<TEntity>, IWriterAsync<TEntity> where TEntity : Entity
{
}
