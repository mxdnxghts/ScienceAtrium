using ScienceAtrium.Domain.Entities;

namespace ScienceAtrium.Application.Common.Interfaces.Base;

public interface IRepository<TEntity> : IBase<TEntity>, IReader<TEntity>, IWriter<TEntity>, IDisposable where TEntity : Entity
{
}
