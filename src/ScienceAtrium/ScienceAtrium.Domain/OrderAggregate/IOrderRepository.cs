using ScienceAtrium.Domain.Entities;
using ScienceAtrium.Domain.Entities.Interfaces;

namespace ScienceAtrium.Domain.OrderAggregate;

public interface IOrderRepository<TEntity> : IRepository<TEntity>, IRepositoryAsync<TEntity> where TEntity : Entity
{
}
