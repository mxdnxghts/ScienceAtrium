using ScienceAtrium.Application.Common.Interfaces.Base;
using ScienceAtrium.Domain.Entities;

namespace ScienceAtrium.Application.Common.Interfaces;
public interface IWorkTemplateRepository<TEntity> : IRepository<TEntity>, IRepositoryAsync<TEntity> where TEntity : Entity
{
}
