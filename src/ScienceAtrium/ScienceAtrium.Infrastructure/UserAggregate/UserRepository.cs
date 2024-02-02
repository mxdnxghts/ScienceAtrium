using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using ScienceAtrium.Domain.Exceptions;
using ScienceAtrium.Domain.RootAggregate.Interfaces;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Infrastructure.Extensions;
using Serilog;
using System.Linq.Expressions;

namespace ScienceAtrium.Infrastructure.UserAggregate;

/// <summary>
/// Represents implementation of interface <see cref="IUserRepository{TEntity}"/>
/// </summary>
/// <typeparam name="TUser">entity that  has a parent class <see cref="User"/></typeparam>
/// <remarks>
/// You have to use any type of <typeparamref name="TUser"/> that 's child of <see cref="User"/> but  not the <see cref="User"/>
/// </remarks>
public sealed class UserRepository<TUser> : IUserRepository<TUser>
    where TUser : User
{
    private readonly ApplicationContext _context;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache;
    private DbSet<TUser> Users { get; }

    public UserRepository(ApplicationContext context, ILogger logger, IMapper mapper, IDistributedCache cache)
    {
        _context = context;
        _logger = logger;
        _mapper = mapper;
        _cache = cache;
        Users = _context.Set<TUser>();
    }
    public IQueryable<TUser> All => Users
        .Include(x => x.Orders)
        .ThenInclude(x => x.WorkTemplatesLink)
        .ThenInclude(x => x.WorkTemplate)
        .ThenInclude(x => x.Subject)
        .AsNoTracking();

    public int Create(TUser entity)
    {
        if (entity is null)
        {
            throw new ValidationException("An error has occurred while creating an entity",
                new ArgumentNullException(nameof(entity)));
        }

        if (Exist(predicate: x => x.Id == entity.Id))
            throw new CreationException(entity.Id);

        Users.Add(entity);
        if (entity.Orders?.Count > 0)
            _context.Orders.UpdateRange(entity.Orders);

        return _context.TrySaveChanges(_logger);
    }

    public async Task<int> CreateAsync(TUser entity, CancellationToken cancellationToken = default)
    {
        if (entity is null)
        {
            throw new ValidationException("An error has occurred while creating an entity",
                new ArgumentNullException(nameof(entity)));
        }

        if (await ExistAsync(predicate: x => x.Id == entity.Id, cancellationToken: cancellationToken))
            throw new CreationException(entity.Id);

        Users.Add(entity);
        if (entity.Orders?.Count > 0)
            _context.Orders.UpdateRange(entity.Orders);

        return await _context.TrySaveChangesAsync(_logger, cancellationToken: cancellationToken);
    }

    public int Delete(TUser entity)
    {
        if (!FitsConditions(entity))
            throw new ValidationException(entity.Id);

        Users.Remove(entity);

        if (entity.Orders?.Count > 0)
            _context.Orders.UpdateRange(entity.Orders);
        RemoveUserFromOrders(ref entity);

        return _context.TrySaveChanges(_logger);
    }

    public async Task<int> DeleteAsync(TUser entity, CancellationToken cancellationToken = default)
    {
        if (!await FitsConditionsAsync(entity, cancellationToken))
            throw new ValidationException(entity.Id);

        Users.Remove(entity);

        if (entity.Orders?.Count > 0)
            _context.Orders.UpdateRange(entity.Orders);
        RemoveUserFromOrders(ref entity);

        return await _context.TrySaveChangesAsync(_logger, cancellationToken: cancellationToken);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }

    public bool Exist(Guid? id = null, Expression<Func<TUser, bool>>? predicate = null)
	{
		if (IsInvalidGetExpression(id, predicate))
			return false;

        return All.Any(predicate ?? (x => x.Id == id));
    }

    public async Task<bool> ExistAsync(Guid? id = null, Expression<Func<TUser, bool>>? predicate = null, CancellationToken cancellationToken = default)
	{
        if (IsInvalidGetExpression(id, predicate))
            return false;

        return await All.AnyAsync(predicate ?? (x => x.Id == id), cancellationToken);
    }

    public bool FitsConditions(TUser? entity)
    {
        if (entity is null)
            return false;

        if (!Exist(predicate: x => x.Id == entity.Id))
            return false;

        return true;
    }

    public async Task<bool> FitsConditionsAsync(TUser? entity, CancellationToken cancellationToken = default)
    {
        if (entity is null)
            return false;

        if (!await ExistAsync(predicate: x => x.Id == entity.Id, cancellationToken: cancellationToken))
            return false;

        return true;
    }

    public TUser Get(Guid? id = null, Expression<Func<TUser, bool>>? predicate = null)
    {
		if (IsInvalidGetExpression(id, predicate))
            return _mapper.Map<TUser>(User.Default);

		var cached = _cache.GetRecord<TUser>($"UserCached_{id}");
		if (cached is not null)
            return cached;

        var user = All.FirstOrDefault(predicate ?? (x => x.Id == id))
            ?? _mapper.Map<TUser>(User.Default);

        if (!user.IsEmpty())
            _cache.SetRecord($"UserCached_{user.Id}", user);
        return user;
    }

    public async Task<TUser> GetAsync(Guid? id = null, Expression<Func<TUser, bool>>? predicate = null, CancellationToken cancellationToken = default)
	{
		if (IsInvalidGetExpression(id, predicate))
			return _mapper.Map<TUser>(User.Default);

		var cached = await _cache.GetRecordAsync<TUser>($"UserCached_{id}", cancellationToken: cancellationToken);
		if (cached is not null)
            return cached;

        var user = await All.FirstOrDefaultAsync(predicate ?? (x => x.Id == id), cancellationToken)
            ?? _mapper.Map<TUser>(User.Default);

        if (!user.IsEmpty())
            await _cache.SetRecordAsync($"UserCached_{user.Id}", user, cancellationToken: cancellationToken);

        return user;
    }

    public int Update(TUser entity)
    {
        if (!FitsConditions(entity))
            throw new ValidationException(entity.Id);

        Users.Update(entity);

        if (entity.Orders?.Count > 0)
            _context.Orders.UpdateRange(entity.Orders);

        return _context.TrySaveChanges(_logger);
    }

    public async Task<int> UpdateAsync(TUser entity, CancellationToken cancellationToken = default)
    {
        if (!await FitsConditionsAsync(entity, cancellationToken))
            throw new ValidationException(entity.Id);

        Users.Update(entity);

        if (entity.Orders?.Count > 0)
            _context.Orders.UpdateRange(entity.Orders);

        return await _context.TrySaveChangesAsync(_logger, cancellationToken: cancellationToken);
	}

	private bool IsInvalidGetExpression(Guid? id = null, Expression<Func<TUser, bool>>? predicate = null)
    {
        return (id == Guid.Empty || id is null) && predicate is null;
	}

	/// <summary>
	///
	/// </summary>
	/// <param name="user"></param>
	/// <returns>count of iterations - count of <see cref="User.Orders"/></returns>
	private int RemoveUserFromOrders(ref TUser user)
    {
        foreach (var order in user.Orders)
        {
            if (user.UserType == UserType.Customer)
                order.RemoveCustomer();
            if (user.UserType == UserType.Executor)
                order.RemoveExecutor();
        }
        return user.Orders.Count;
    }
}
