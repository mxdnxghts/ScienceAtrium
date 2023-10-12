using Microsoft.EntityFrameworkCore;
using ScienceAtrium.Application.Common.Interfaces;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Infrastructure.Data;
using Serilog;
using System.Linq.Expressions;

namespace ScienceAtrium.Infrastructure.Repositories.UserAggregation;
public sealed class UserRepository<TUser> : IUserRepository<TUser> where TUser : User
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
    public IQueryable<TUser> All { get; }

    public int Create(TUser entity)
    {
        throw new NotImplementedException();
    }

    public Task<int> CreateAsync(TUser entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public int Delete(TUser entity)
    {
        throw new NotImplementedException();
    }

    public Task<int> DeleteAsync(TUser entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public bool Exist(Expression<Func<TUser, bool>> predicate)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistAsync(Expression<Func<TUser, bool>> predicate, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public bool FitsConditions(TUser? entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> FitsConditionsAsync(TUser? entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public TUser Get(Expression<Func<TUser, bool>> predicate)
    {
        throw new NotImplementedException();
    }

    public Task<TUser> GetAsync(Expression<Func<TUser, bool>> predicate, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public int Update(TUser entity)
    {
        throw new NotImplementedException();
    }

    public Task<int> UpdateAsync(TUser entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
