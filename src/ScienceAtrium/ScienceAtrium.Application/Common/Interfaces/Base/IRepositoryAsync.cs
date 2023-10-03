using ScienceAtrium.Domain.Entities;

namespace ScienceAtrium.Application.Common.Interfaces.Base;

public interface IRepositoryAsync<TEntity> : IBase<TEntity>, IReaderAsync<TEntity>, IWriterAsync<TEntity> where TEntity : Entity
{
}
