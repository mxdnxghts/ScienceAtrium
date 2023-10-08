using Microsoft.EntityFrameworkCore;
using ScienceAtrium.Application.Common.Interfaces;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Domain.WorkTemplateAggregate;
using ScienceAtrium.Infrastructure.Data;
using Serilog;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace ScienceAtrium.Infrastructure.Repositories;
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
        if (entity?.Customer is null || entity?.Executor is null)
            throw new ArgumentNullException(nameof(entity));

        if (Exist(x => x.Id == entity.Id))
            throw new InvalidOperationException();

        return _context.SaveChanges();
    }

    public Task<int> CreateAsync(WorkTemplate entity)
    {
        throw new NotImplementedException();
    }

    public int Delete(WorkTemplate entity)
    {
        throw new NotImplementedException();
    }

    public Task<int> DeleteAsync(WorkTemplate entity)
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

    public Task<bool> ExistAsync(Expression<Func<WorkTemplate, bool>> predicate)
    {
        throw new NotImplementedException();
    }

    public bool FitsConditions(WorkTemplate? entity)
    {
        if (entity?.Customer is null || entity?.Executor is null)
            throw new ArgumentNullException(nameof(entity));

        if (Exist(x => x.Id == entity.Id))
            throw new InvalidOperationException();

        return true;
    }

    public async Task<bool> FitsConditionsAsync(WorkTemplate? entity)
    {
        if (entity?.Customer is null || entity?.Executor is null)
            throw new ArgumentNullException(nameof(entity));

        if (!await ExistAsync(x => x.Id == entity.Id))
            throw new InvalidOperationException();

        return true;
    }


    public WorkTemplate Get(Expression<Func<WorkTemplate, bool>> predicate)
    {
        return _context.WorkTemplates.FirstOrDefault(predicate);
    }

    public async Task<WorkTemplate> GetAsync(Expression<Func<WorkTemplate, bool>> predicate)
    {
        return await _context.WorkTemplates.FirstOrDefaultAsync(predicate);
    }

    public int Update(WorkTemplate entity)
    {
        throw new NotImplementedException();
    }

    public Task<int> UpdateAsync(WorkTemplate entity)
    {
        throw new NotImplementedException();
    }
}



