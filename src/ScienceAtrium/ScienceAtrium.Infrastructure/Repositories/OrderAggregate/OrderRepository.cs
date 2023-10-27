using Microsoft.EntityFrameworkCore;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Infrastructure.Extensions;
using Serilog;
using System.Linq.Expressions;
using ScienceAtrium.Domain.Exceptions;

namespace ScienceAtrium.Infrastructure.Repositories.OrderAggregate;
public sealed class OrderRepository : IOrderRepository<Order>
{
    private readonly ApplicationContext _context;
    private readonly ILogger _logger;

    public OrderRepository(ApplicationContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    public IQueryable<Order> All => _context.Orders
        .Include(x => x.Customer.FormerOrders)
        .Include(x => x.Executor.DoneOrders)
        .AsNoTracking();

    public int Create(Order entity)
    {
        if (entity?.Customer is null || entity?.Executor is null)
            throw new ValidationException();

        if (Exist(x => x.Id == entity.Id))
            throw new EntityNotFoundException();

        entity.Customer.UpdateDetails(entity);
        entity.Executor.UpdateDetails(entity);

        _context.Orders.Add(entity);
        _context.Users.UpdateRange(entity.Customer, entity.Executor);

        return _context.TrySaveChanges(_logger);
    }

    public async Task<int> CreateAsync(Order entity, CancellationToken cancellationToken = default)
    {
        if (entity?.Customer is null || entity?.Executor is null)
            throw new ValidationException();

        if (await ExistAsync(x => x.Id == entity.Id, cancellationToken))
            throw new EntityNotFoundException();

        entity.Customer.UpdateDetails(entity);
        entity.Executor.UpdateDetails(entity);

        _context.Orders.Add(entity);
        _context.Users.UpdateRange(entity.Customer, entity.Executor);
        
        return await _context.TrySaveChangesAsync(_logger, cancellationToken: cancellationToken);
    }

    public int Delete(Order entity)
    {
        if (!FitsConditions(entity))
            throw new ValidationException();

        entity.Customer.UpdateDetails(null);
        entity.Executor.UpdateDetails(null);

        _context.Orders.Remove(entity);
        _context.Users.UpdateRange(entity.Customer, entity.Executor);

        return _context.TrySaveChanges(_logger);
    }

    public async Task<int> DeleteAsync(Order entity, CancellationToken cancellationToken = default)
    {
        if (!await FitsConditionsAsync(entity, cancellationToken))
            throw new ValidationException();

        entity.Customer.UpdateDetails(null);
        entity.Executor.UpdateDetails(null);

        _context.Orders.Remove(entity);
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
        if (entity?.Customer is null || entity?.Executor is null)
            return false;

        if (entity.IsEmpty())
            return false;

        if (!Exist(x => x.Id == entity.Id))
            return false;

        return true;
    }

    public async Task<bool> FitsConditionsAsync(Order? entity, CancellationToken cancellationToken = default)
    {
        if (entity?.Customer is null || entity?.Executor is null)
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

        entity.Customer.UpdateDetails(entity);
        entity.Executor.UpdateDetails(entity);

        _context.Orders.Update(entity);
        _context.Users.UpdateRange(entity.Customer, entity.Executor);

        return _context.TrySaveChanges(_logger);
    }

    public async Task<int> UpdateAsync(Order entity, CancellationToken cancellationToken = default)
    {
        if (!await FitsConditionsAsync(entity, cancellationToken))
            throw new ValidationException();

        entity.Customer.UpdateDetails(entity);
        entity.Executor.UpdateDetails(entity);

        _context.Orders.Update(entity);
        _context.Users.UpdateRange(entity.Customer, entity.Executor);

        return await _context.TrySaveChangesAsync(_logger, cancellationToken: cancellationToken);
    }
}
