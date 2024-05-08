namespace ScienceAtrium.Domain.RootAggregate.Interfaces;

public interface IRepository<TEntity> : IBase<TEntity>, IReader<TEntity>, IWriter<TEntity>, IDisposable where TEntity : Entity
{
}