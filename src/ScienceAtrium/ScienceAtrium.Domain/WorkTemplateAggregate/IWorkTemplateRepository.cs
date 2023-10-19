using ScienceAtrium.Domain.Entities;
using ScienceAtrium.Domain.Entities.Interfaces;

namespace ScienceAtrium.Domain.WorkTemplateAggregate;
public interface IWorkTemplateRepository<TEntity> : IRepository<TEntity>, IRepositoryAsync<TEntity> where TEntity : Entity
{
}
