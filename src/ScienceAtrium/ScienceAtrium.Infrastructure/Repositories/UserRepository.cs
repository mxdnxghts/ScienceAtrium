using ScienceAtrium.Application.Common.Interfaces;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Infrastructure.Data;
using Serilog;

namespace ScienceAtrium.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository<User>
{
    private readonly ApplicationContext _context;
    private readonly ILogger _logger;

    public UserRepository(ApplicationContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
    }
public IQueryable<User> All => _context.Users;

    public int Create(User entity)
    {
        throw new NotImplementedException();
    }

    public Task<int> CreateAsync(User entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public int Delete(User entity)
    {
        throw new NotImplementedException();
    }

    public Task<int> DeleteAsync(User entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public bool Exist(System.Linq.Expressions.Expression<Func<User, bool>> predicate)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistAsync(System.Linq.Expressions.Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public bool FitsConditions(User? entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> FitsConditionsAsync(User? entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public User Get(System.Linq.Expressions.Expression<Func<User, bool>> predicate)
    {
        throw new NotImplementedException();
    }

    public Task<User> GetAsync(System.Linq.Expressions.Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public int Update(User entity)
    {
        throw new NotImplementedException();
    }

    public Task<int> UpdateAsync(User entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}