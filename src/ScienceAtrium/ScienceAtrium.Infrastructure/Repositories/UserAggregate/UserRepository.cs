using Microsoft.EntityFrameworkCore;
using ScienceAtrium.Application.Common.Exceptions;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Infrastructure.Extensions;
using Serilog;
using System.Linq.Expressions;

namespace ScienceAtrium.Infrastructure.Repositories.UserAggregation;
public sealed class UserRepository<TUser> : IUserRepository<TUser>
    where TUser : User
{
    private readonly ApplicationContext _context;
    private readonly ILogger _logger;
    private DbSet<TUser> Users { get; }

    public UserRepository(ApplicationContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
        Users = _context.Set<TUser>();
    }
    public IQueryable<TUser> All => Users.Include(x => x.CurrentOrder).AsNoTracking();

    public int Create(TUser entity)
    {
        if (entity is null)
        {
            throw new ValidationException("An error has occurred while creating an entity",
                new ArgumentNullException(nameof(entity)));
        }

        if (Exist(x => x.Id == entity.Id))
            throw new CreationException(entity.Id);

        Users.Add(entity);
        if (entity.CurrentOrder is not null)
            _context.Orders.Update(entity.CurrentOrder);

        return _context.TrySaveChanges(_logger);
    }

    public async Task<int> CreateAsync(TUser entity, CancellationToken cancellationToken = default)
    {
        if (entity is null)
        {
            throw new ValidationException("An error has occurred while creating an entity",
                new ArgumentNullException(nameof(entity)));
        }

        if (await ExistAsync(x => x.Id == entity.Id, cancellationToken))
            throw new CreationException(entity.Id);

        Users.Add(entity);
        if (entity.CurrentOrder is not null)
            _context.Orders.Update(entity.CurrentOrder);

        return await _context.TrySaveChangesAsync(_logger, cancellationToken: cancellationToken);
    }

    public int Delete(TUser entity)
    {
        if (!FitsConditions(entity))
            throw new ValidationException(entity.Id);

        Users.Remove(entity);
        if (entity.CurrentOrder is not null)
            _context.Orders.Update(entity.CurrentOrder);
        entity.UpdateDetails(null);

        return _context.TrySaveChanges(_logger);
    }

    public async Task<int> DeleteAsync(TUser entity, CancellationToken cancellationToken = default)
    {
        if (!await FitsConditionsAsync(entity, cancellationToken))
            throw new ValidationException(entity.Id);

        Users.Remove(entity);
        if (entity.CurrentOrder is not null)
            _context.Orders.Update(entity.CurrentOrder);
        entity.UpdateDetails(null);

        return await _context.TrySaveChangesAsync(_logger, cancellationToken: cancellationToken);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }

    public bool Exist(Expression<Func<TUser, bool>> predicate)
    {
        return All.Any(predicate);
    }

    public async Task<bool> ExistAsync(Expression<Func<TUser, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await All.AnyAsync(predicate, cancellationToken);
    }

    public bool FitsConditions(TUser? entity)
    {
        if (entity is null)
            return false;

        if (!Exist(x => x.Id == entity.Id))
            return false;

        return true;
    }

    public async Task<bool> FitsConditionsAsync(TUser? entity, CancellationToken cancellationToken = default)
    {
        if (entity is null)
            return false;

        if (!await ExistAsync(x => x.Id == entity.Id, cancellationToken))
            return false;

        return true;
    }

    public TUser Get(Expression<Func<TUser, bool>> predicate)
    {
        return All.FirstOrDefault(predicate) ?? User.MapTo<TUser>(User.Default);
    }

    public async Task<TUser> GetAsync(Expression<Func<TUser, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await All.FirstOrDefaultAsync(predicate, cancellationToken) ?? User.MapTo<TUser>(User.Default);
    }

    public int Update(TUser entity)
    {
        if (!FitsConditions(entity))
            throw new ValidationException(entity.Id);

        Users.Update(entity);
        if (entity.CurrentOrder is not null)
            _context.Orders.Update(entity.CurrentOrder);

        return _context.TrySaveChanges(_logger);
    }

    public async Task<int> UpdateAsync(TUser entity, CancellationToken cancellationToken = default)
    {
        if (!await FitsConditionsAsync(entity, cancellationToken))
            throw new ValidationException(entity.Id);

        Users.Update(entity);
        if (entity.CurrentOrder is not null)
            _context.Orders.Update(entity.CurrentOrder);

        return await _context.TrySaveChangesAsync(_logger, cancellationToken: cancellationToken);
    }
}
