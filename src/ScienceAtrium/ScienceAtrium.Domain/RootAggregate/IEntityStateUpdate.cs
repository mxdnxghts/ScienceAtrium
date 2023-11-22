using Microsoft.EntityFrameworkCore;

namespace ScienceAtrium.Domain.RootAggregate;
public interface IEntityStateUpdate<TEntity> where TEntity : Entity
{
	/// <summary>
	/// do all changes of  <paramref name="entity"/>  and its nested entities that need to save
	/// </summary>
	/// <param name="entity"></param>
	/// <param name="entityState"></param>
	/// <returns></returns>
	TEntity UpdateState(TEntity entity, EntityState entityState);

	/// <summary>
	/// Update state of <paramref name="entity"/>
	/// </summary>
	/// <param name="entity"></param>
	/// <param name="entityState"></param>
	/// <returns></returns>
	TEntity UpdateEntity(TEntity entity, EntityState entityState);
}
