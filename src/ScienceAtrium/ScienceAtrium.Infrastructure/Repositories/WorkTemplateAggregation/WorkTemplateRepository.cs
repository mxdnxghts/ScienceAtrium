using Microsoft.EntityFrameworkCore;
using ScienceAtrium.Application.Common.Exceptions;
using ScienceAtrium.Application.Common.Interfaces;
using ScienceAtrium.Domain.WorkTemplateAggregate;
using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Infrastructure.Extensions;
using Serilog;
using System.Linq.Expressions;

namespace ScienceAtrium.Infrastructure.Repositories.WorkTemplateAggregation;
public sealed class WorkTemplateRepository : IWorkTemplateRepository<WorkTemplate>
{
    private readonly ApplicationContext _context;
    private readonly ILogger _logger;

    public WorkTemplateRepository(ApplicationContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    public IQueryable<WorkTemplate> All => _context.WorkTemplates;

    public int Create(WorkTemplate entity)
    {
        if (entity?.Subject is null)
            throw new ArgumentNullException(nameof(entity));

        if (Exist(x => x.Id == entity.Id))
            throw new EntityNotFoundException();

        _context.WorkTemplates.Add(entity);

        return _context.TrySaveChanges(_logger);
    }

    public Task<int> CreateAsync(WorkTemplate entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public int Delete(WorkTemplate entity)
    {
        throw new NotImplementedException();
    }

    public Task<int> DeleteAsync(WorkTemplate entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public bool Exist(Expression<Func<WorkTemplate, bool>> predicate)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistAsync(Expression<Func<WorkTemplate, bool>> predicate, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public bool FitsConditions(WorkTemplate? entity)
    {
        if (entity?.Subject is null)
            return false;

        if (Exist(x => x.Id == entity.Id))
            return false;

        return true;
    }

    public async Task<bool> FitsConditionsAsync(WorkTemplate? entity, CancellationToken cancellationToken = default)
    {
        if (entity?.Subject is null)
            return false;

        if (!await ExistAsync(x => x.Id == entity.Id))
            return false;

        return true;
    }


    public WorkTemplate Get(Expression<Func<WorkTemplate, bool>> predicate)
    {
        return _context.WorkTemplates.FirstOrDefault(predicate);
    }

    public async Task<WorkTemplate> GetAsync(Expression<Func<WorkTemplate, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.WorkTemplates.FirstOrDefaultAsync(predicate);
    }

    public int Update(WorkTemplate entity)
    {
        throw new NotImplementedException();
    }

    public Task<int> UpdateAsync(WorkTemplate entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}



