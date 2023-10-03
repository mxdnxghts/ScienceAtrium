using ScienceAtrium.Application.Common.Interfaces.Base;
using ScienceAtrium.Domain.Entities;

namespace ScienceAtrium.Application.Common.Interfaces;

public interface IOrderRepository<TEntity> : IRepository<TEntity>, IRepositoryAsync<TEntity> where TEntity : Entity
{
}
