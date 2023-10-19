using ScienceAtrium.Domain.Entities;
using ScienceAtrium.Domain.Entities.Interfaces;

namespace ScienceAtrium.Domain.UserAggregate;
public interface IUserRepository<TEntity> : IRepository<TEntity>, IRepositoryAsync<TEntity> where TEntity : Entity
{
}
