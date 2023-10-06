using Microsoft.EntityFrameworkCore;
using ScienceAtrium.Application.Common.Interfaces;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Infrastructure.Data;
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

    public IQueryable<Order> All => _context.Orders;

    public int Create(Order entity)
    {
        if (entity?.Customer is null || entity?.Executor is null)
            throw new ArgumentNullException(nameof(entity));

        if (Exist(x => x.Id == entity.Id))
            throw new InvalidOperationException();

        using var transaction = _context.Database.CurrentTransaction
            ?? _context.Database.BeginTransaction(System.Data.IsolationLevel.RepeatableRead);

        _context.Orders.Add(entity);

        try
        {
            transaction.Commit();
            return _context.SaveChanges();
        }
        catch (Exception)
        {
            transaction.Rollback();
            return -1;
        }
    }

    public async Task<int> CreateAsync(Order entity)
    {
        throw new NotImplementedException();
    }

    public int Delete(Order entity)
    {
        throw new NotImplementedException();
    }

    public async Task<int> DeleteAsync(Order entity)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        _context?.Dispose();
    }

    public bool Exist(Expression<Func<Order, bool>> predicate)
    {
        return _context.Orders.Any(predicate);
    }

    public async Task<bool> ExistAsync(Expression<Func<Order, bool>> predicate)
    {
        return await _context.Orders.AnyAsync(predicate);
    }

    public bool FitsConditions(Order? entity)
    {
        if (entity?.Customer is null || entity?.Executor is null)
            throw new ArgumentNullException(nameof(entity));

        if (!Exist(x => x.Id == entity.Id))
            throw new InvalidOperationException();

        return true;
    }

    public async Task<bool> FitsConditionsAsync(Order? entity)
    {
        if (entity?.Customer is null || entity?.Executor is null)
            throw new ArgumentNullException(nameof(entity));

        if (!await ExistAsync(x => x.Id == entity.Id))
            throw new InvalidOperationException();

        return true;
    }

    public Order Get(Expression<Func<Order, bool>> predicate)
    {
        return _context.Orders.FirstOrDefault(predicate);
    }

    public async Task<Order> GetAsync(Expression<Func<Order, bool>> predicate)
    {
        return await _context.Orders.FirstOrDefaultAsync(predicate);
    }

    public int Update(Order entity)
    {
        throw new NotImplementedException();
    }

    public async Task<int> UpdateAsync(Order entity)
    {
        throw new NotImplementedException();
    }
}
