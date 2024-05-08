namespace ScienceAtrium.Domain.RootAggregate.Interfaces;

public interface IRepositoryAsync<TEntity> : IBase<TEntity>, IReaderAsync<TEntity>, IWriterAsync<TEntity> where TEntity : Entity
{
}