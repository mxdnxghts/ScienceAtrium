using Microsoft.EntityFrameworkCore;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Infrastructure.Extensions;
using Serilog;
using System.Linq.Expressions;
using ScienceAtrium.Domain.Exceptions;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;
using ScienceAtrium.Domain.UserAggregate.ExecutorAggregate;
using ScienceAtrium.Domain.RootAggregate;

namespace ScienceAtrium.Infrastructure.Repositories.OrderAggregate;
public sealed class OrderRepository : IOrderRepository<Order>, IEntityStateUpdate<Order>
{
    private readonly ApplicationContext _context;
    private readonly ILogger _logger;

    public OrderRepository(ApplicationContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    public IQueryable<Order> All => _context.Orders
        .Include(x => x.Customer.FormedOrders)
        .Include(x => x.Executor.DoneOrders)
        .Include(x => x.WorkTemplates).ThenInclude(x => x.Subject)
        .AsNoTracking();

    public int Create(Order entity)
    {
        if (entity?.Customer is null)
            throw new ValidationException();

        if (Exist(x => x.Id == entity.Id))
            throw new EntityNotFoundException();

        entity.Customer.UpdateCurrentOrder(entity);
        entity.Executor?.UpdateCurrentOrder(entity);

        _context.Orders.Add(entity);
        _context.WorkTemplates.AttachRange(entity.WorkTemplates);
        _context.Users.UpdateRange(entity.Customer, entity.Executor);

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

        entity.Customer.UpdateCurrentOrder(null);
        entity.Executor?.UpdateCurrentOrder(null);

        _context.Orders.Remove(entity);
		_context.WorkTemplates.AttachRange(entity.WorkTemplates);
		_context.Users.UpdateRange(entity.Customer, entity.Executor);

        return _context.TrySaveChanges(_logger);
    }

    public async Task<int> DeleteAsync(Order entity, CancellationToken cancellationToken = default)
    {
        if (!await FitsConditionsAsync(entity, cancellationToken))
            throw new ValidationException();

        entity.Customer.UpdateCurrentOrder(null);
        entity.Executor?.UpdateCurrentOrder(null);

        _context.Orders.Remove(entity);
		_context.WorkTemplates.AttachRange(entity.WorkTemplates);
		_context.Users.UpdateRange(entity.Customer, entity.Executor);

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

        entity.Customer.UpdateCurrentOrder(entity);
        entity.Executor?.UpdateCurrentOrder(entity);

        _context.Orders.Update(entity);
		_context.WorkTemplates.AttachRange(entity.WorkTemplates);
		_context.Users.UpdateRange(entity.Customer, entity.Executor);

        return _context.TrySaveChanges(_logger);
    }

    public async Task<int> UpdateAsync(Order entity, CancellationToken cancellationToken = default)
    {
        if (!await FitsConditionsAsync(entity, cancellationToken))
            throw new ValidationException();

        entity.Customer.UpdateCurrentOrder(entity);
        entity.Executor?.UpdateCurrentOrder(entity);

        
		_context.WorkTemplates.AttachRange(entity.WorkTemplates);
		_context.Users.UpdateRange(entity.Customer, entity.Executor);

        return await _context.TrySaveChangesAsync(_logger, cancellationToken: cancellationToken);
    }

	public Order UpdateEntity(Order entity, EntityState entityState)
	{
		if (entityState == EntityState.Added)
		{
			_context.Orders.Add(entity);
		}
        else if (entityState == EntityState.Modified)
        {
            _context.Orders.Update(entity);
        }
        else if (entityState == EntityState.Deleted)
        {
            _context.Orders.Remove(entity);
        }

        return entity;
	}

	public Order UpdateState(Order entity, EntityState entityState)
	{
		entity.Customer.UpdateCurrentOrder(entity);
		entity.Executor?.UpdateCurrentOrder(entity);

        UpdateEntity(entity, entityState);
		_context.WorkTemplates.AttachRange(entity.WorkTemplates);
		_context.Users.Update(entity.Customer);

        if (entity.Executor is not null)
            _context.Users.Update(entity.Executor);

		_context.Subjects.AttachRange(entity.WorkTemplates.Select(x => x.Subject));

		var state = _context.Entry(entity.WorkTemplates.ToList()[0].Subject);
        return entity;
	}
}
