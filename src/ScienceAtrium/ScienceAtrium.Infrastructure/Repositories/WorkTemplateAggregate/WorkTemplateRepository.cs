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

    public IQueryable<WorkTemplate> All => _context.WorkTemplates
        .Include(x => x.Subject).AsNoTracking();

    public int Create(WorkTemplate entity)
    {
        if (entity?.Subject is null)
            throw new ValidationException();

        if (Exist(x => x.Id == entity.Id))
            throw new EntityNotFoundException();

        _context.WorkTemplates.Add(entity);
        

        return _context.TrySaveChanges(_logger);
    }

    public async Task<int> CreateAsync(WorkTemplate entity, CancellationToken cancellationToken = default)
    {
        if (entity?.Subject is null)
            throw new ValidationException();

        if (Exist(x => x.Id == entity.Id))
            throw new EntityNotFoundException();

        _context.WorkTemplates.Add(entity);

        return await _context.TrySaveChangesAsync(_logger, cancellationToken: cancellationToken);
    }

    public int Delete(WorkTemplate entity)
    {
        if (!FitsConditions(entity))
            throw new ValidationException();

        _context.WorkTemplates.Remove(entity);
        _context.Subjects.Update(entity.Subject);

        return _context.TrySaveChanges(_logger);
    }

    public async Task<int> DeleteAsync(WorkTemplate entity, CancellationToken cancellationToken = default)
    {
        if (!await FitsConditionsAsync(entity))
            throw new ValidationException();

        _context.WorkTemplates.Remove(entity);
        _context.Subjects.Update(entity.Subject);

        return await _context.TrySaveChangesAsync(_logger, cancellationToken: cancellationToken);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }

    public bool Exist(Expression<Func<WorkTemplate, bool>> predicate)
    {
        return All.Any(predicate);
    }

    public Task<bool> ExistAsync(Expression<Func<WorkTemplate, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return All.AnyAsync(predicate, cancellationToken);
    }

    public bool FitsConditions(WorkTemplate? entity)
    {
        if (entity?.Subject is null)
            return false;

        if (!Exist(x => x.Id == entity.Id))
            return false;

        return true;
    }

    public async Task<bool> FitsConditionsAsync(WorkTemplate? entity, CancellationToken cancellationToken = default)
    {
        if (entity?.Subject is null)
            return false;

        if (!await ExistAsync(x => x.Id == entity.Id, cancellationToken))
            return false;

        return true;
    }


    public WorkTemplate Get(Expression<Func<WorkTemplate, bool>> predicate)
    {
        return All.FirstOrDefault(predicate) ?? WorkTemplate.Default;
    }

    public async Task<WorkTemplate> GetAsync(Expression<Func<WorkTemplate, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await All.FirstOrDefaultAsync(predicate, cancellationToken) ?? WorkTemplate.Default;
    }

    public int Update(WorkTemplate entity)
    {
        if (!FitsConditions(entity))
            throw new ValidationException();

        _context.WorkTemplates.Update(entity);
        _context.Subjects.Update(entity.Subject);

        return _context.TrySaveChanges(_logger);
    }

    public async Task<int> UpdateAsync(WorkTemplate entity, CancellationToken cancellationToken = default)
    {
        if (!await FitsConditionsAsync(entity, cancellationToken))
            throw new ValidationException();

        _context.WorkTemplates.Remove(entity);
        _context.Subjects.Update(entity.Subject);

        return await _context.TrySaveChangesAsync(_logger, cancellationToken: cancellationToken);
    }
}



