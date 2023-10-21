using ScienceAtrium.Domain.RootAggregate;
using ScienceAtrium.Domain.RootAggregate.Interfaces;

namespace ScienceAtrium.Domain.UserAggregate;
public interface IUserRepository<TEntity> : IRepository<TEntity>, IRepositoryAsync<TEntity> where TEntity : Entity
{
}
