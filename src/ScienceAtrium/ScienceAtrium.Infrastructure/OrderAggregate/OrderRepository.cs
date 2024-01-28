using Microsoft.EntityFrameworkCore;
using ScienceAtrium.Domain.Exceptions;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Domain.RootAggregate;
using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Infrastructure.Extensions;
using Serilog;
using System.Linq.Expressions;

namespace ScienceAtrium.Infrastructure.OrderAggregate;
public sealed class OrderRepository : IOrderRepository<Order>, IEntityStateUpdate<Order, bool>
{
    private readonly ApplicationContext _context;
    private readonly ILogger _logger;

    public OrderRepository(ApplicationContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    public IQueryable<Order> All => _context.Orders
        .Include(x => x.Customer)
        .Include(x => x.Executor)
        .Include(x => x.WorkTemplatesLink).ThenInclude(x => x.WorkTemplate).ThenInclude(x => x.Subject);

    public int Create(Order entity)
    {
        if (entity?.Customer is null)
            throw new ValidationException();

        if (Exist(x => x.Id == entity.Id))
            throw new EntityNotFoundException();

        UpdateState(entity, EntityState.Added);

        return _context.TrySaveChanges(_logger);
    }

    public async Task<int> CreateAsync(Order entity, CancellationToken cancellationToken = default)
    {
        if (entity?.Customer is null)
            throw new ValidationException();

        if (await ExistAsync(x => x.Id == entity.Id, cancellationToken))
            throw new EntityNotFoundException();

        UpdateState(entity, EntityState.Added);

        return await _context.TrySaveChangesAsync(_logger, cancellationToken: cancellationToken);
    }

    public int Delete(Order entity)
    {
        if (!FitsConditions(entity))
            throw new ValidationException();

        UpdateState(entity, EntityState.Deleted);

        return _context.TrySaveChanges(_logger);
    }

    public async Task<int> DeleteAsync(Order entity, CancellationToken cancellationToken = default)
    {
        if (!await FitsConditionsAsync(entity, cancellationToken))
            throw new ValidationException();

        UpdateState(entity, EntityState.Deleted);

        return await _context.TrySaveChangesAsync(_logger, cancellationToken: cancellationToken);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }

    public bool Exist(Expression<Func<Order, bool>> predicate)
    {
        return All.Any(predicate);
    }

    public async Task<bool> ExistAsync(Expression<Func<Order, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await All.AnyAsync(predicate, cancellationToken);
    }

    public bool FitsConditions(Order? entity)
    {
        if (entity?.Customer is null)
            return false;

        if (entity.IsEmpty())
            return false;

        if (!Exist(x => x.Id == entity.Id))
            return false;

        return true;
    }

    public async Task<bool> FitsConditionsAsync(Order? entity, CancellationToken cancellationToken = default)
    {
        if (entity?.Customer is null)
            return false;

        if (entity.IsEmpty())
            return false;

        if (!await ExistAsync(x => x.Id == entity.Id, cancellationToken))
            return false;

        return true;
    }

    public Order Get(Expression<Func<Order, bool>> predicate)
    {
        return All.FirstOrDefault(predicate) ?? Order.Default;
    }

    public async Task<Order> GetAsync(Expression<Func<Order, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await All.FirstOrDefaultAsync(predicate, cancellationToken) ?? Order.Default;
    }

    public int Update(Order entity)
	{
		if (!FitsConditions(entity))
			throw new ValidationException();

		if (!UpdateState(entity, EntityState.Modified))
			return -1;

		return _context.TrySaveChanges(_logger);
	}

	public async Task<int> UpdateAsync(Order entity, CancellationToken cancellationToken = default)
	{
		if (!await FitsConditionsAsync(entity, cancellationToken))
			throw new ValidationException();

		if (!UpdateState(entity, EntityState.Modified))
			return -1;

		return await _context.TrySaveChangesAsync(_logger, cancellationToken: cancellationToken);
	}

	public bool UpdateEntity(Order entity, EntityState entityState)
	{
		if (entityState == EntityState.Added)
		{
			_context.Orders.Add(entity);
			entity.Customer.AddOrder(entity);
			entity.Executor?.AddOrder(entity);
			_context.OrderWorkTemplates.AddRange(entity.WorkTemplatesLink);

		}
		else if (entityState == EntityState.Modified)
		{
			_context.Orders.Update(entity);
			entity.Customer.UpdateOrder(x => x.Id == entity.Id, entity);
			entity.Executor?.UpdateOrder(x => x.Id == entity.Id, entity);
			TrackOrderWorkTemplatesOnModified(entity);
		}
		else if (entityState == EntityState.Deleted)
		{
			_context.Orders.Remove(entity);
			entity.Customer.RemoveOrder(x => x.Id == entity.Id);
			entity.Executor?.RemoveOrder(x => x.Id == entity.Id);
			_context.OrderWorkTemplates.RemoveRange(entity.WorkTemplatesLink);
		}

		return true;
	}

	public bool UpdateState(Order entity, EntityState entityState)
	{
        try
		{
			if (entity.Executor is not null)
				_context.Users.Update(entity.Executor);

			_context.Users.Update(entity.Customer);

			UpdateEntity(entity, entityState);

			_context.WorkTemplates.AttachRange(entity.WorkTemplates);
			_context.Subjects.AttachRange(entity.WorkTemplates?.Select(x => x.Subject));

			DetachOrderWorkTemplates(entity.WorkTemplatesLink);
		}
        catch (Exception ex)
        {
            _logger.Error(ex.Message + ex.StackTrace);
            _context.ChangeTracker.Clear();
            return false;
        }   

		return true;
	}

	private void TrackOrderWorkTemplatesOnModified(Order order)
	{
		_context.TrackEntities(order.WorkTemplatesLink.ToArray(), EntityState.Detached);

		_context.OrderWorkTemplates.AddRange(
			GetOrderWorkTemplatesByEntityState(order.WorkTemplatesLink, EntityState.Added));

		_context.OrderWorkTemplates.RemoveRange(
			GetOrderWorkTemplatesByEntityState(order.WorkTemplatesLink, EntityState.Deleted));

		_context.OrderWorkTemplates.UpdateRange(
			GetOrderWorkTemplatesByEntityState(order.WorkTemplatesLink, EntityState.Modified));
	}

	private IEnumerable<OrderWorkTemplate> GetOrderWorkTemplatesByEntityState(IEnumerable<OrderWorkTemplate> orderWorkTemplates,
		EntityState entityState)
	{
		return orderWorkTemplates.Where(owt => owt.EntityState == entityState);
	}

	private void DetachOrderWorkTemplates(IEnumerable<OrderWorkTemplate> orderWorkTemplates)
	{
		foreach (var orderWorkTemplate in orderWorkTemplates)
			orderWorkTemplate.EntityState = EntityState.Detached;
	}
}
