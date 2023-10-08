using Microsoft.EntityFrameworkCore;
using ScienceAtrium.Application.Common.Exceptions;
using ScienceAtrium.Application.Common.Interfaces;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Infrastructure.Extensions;
using Serilog;
using System.Linq.Expressions;

namespace ScienceAtrium.Infrastructure.Repositories;
public sealed class OrderRepository : IOrderRepository<Order>
{
    private readonly ApplicationContext _context;
    private readonly ILogger _logger;

    public OrderRepository(ApplicationContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    public IQueryable<Order> All => _context.Orders.AsNoTracking();

    public int Create(Order entity)
    {
        if (entity?.Customer is null || entity?.Executor is null)
            throw new ArgumentNullException($"Argument {{{nameof(entity)}}} or its inner fields is null.");

        if (Exist(x => x.Id == entity.Id))
            throw new EntityNotFoundException();
        
        _context.Orders.Add(entity);

        return _context.TrySaveChanges(_logger);
    }

    public async Task<int> CreateAsync(Order entity, CancellationToken cancellationToken = default)
    {
        if (entity?.Customer is null || entity?.Executor is null)
            throw new ValidationException();

        if (await ExistAsync(x => x.Id == entity.Id))
            throw new EntityNotFoundException();
        
        _context.Orders.Add(entity);

        return await _context.TrySaveChangesAsync(_logger, cancellationToken: cancellationToken);
    }

    public int Delete(Order entity)
    {
        if (!FitsConditions(entity))
            throw new ValidationException();

        _context.Orders.Remove(entity);
        _context.Users.UpdateRange(entity.Customer, entity.Executor);

        return _context.TrySaveChanges(_logger);
    }

    public async Task<int> DeleteAsync(Order entity, CancellationToken cancellationToken = default)
    {
        if (!await FitsConditionsAsync(entity))
            throw new ValidationException();

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

        if (!Exist(x => x.Id == entity.Id))
            return false;

        return true;
    }

    public async Task<bool> FitsConditionsAsync(Order? entity, CancellationToken cancellationToken = default)
    {
        if (entity?.Customer is null || entity?.Executor is null)
            return false;

        if (!await ExistAsync(x => x.Id == entity.Id, cancellationToken))
            return false;

        return true;
    }

    public Order Get(Expression<Func<Order, bool>> predicate)
    {
        return All.FirstOrDefault(predicate);
    }

    public async Task<Order> GetAsync(Expression<Func<Order, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await All.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public int Update(Order entity)
    {
        if (!FitsConditions(entity))
            throw new ValidationException();

        _context.Orders.Update(entity);
        _context.Users.UpdateRange(entity.Customer, entity.Executor);

        return _context.TrySaveChanges(_logger);
    }

    public async Task<int> UpdateAsync(Order entity, CancellationToken cancellationToken = default)
    {
        if (!await FitsConditionsAsync(entity))
            throw new ValidationException();

        _context.Orders.Update(entity);
        _context.Users.UpdateRange(entity.Customer, entity.Executor);

        return await _context.TrySaveChangesAsync(_logger, cancellationToken: cancellationToken);
    }
}
