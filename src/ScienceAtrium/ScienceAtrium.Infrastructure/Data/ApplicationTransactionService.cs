using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ScienceAtrium.Infrastructure.Data;
public sealed class ApplicationTransactionService(ApplicationContext _context, Serilog.ILogger _logger)
{
    public async Task BeginTransactionScopeAsync(
        Func<Task> isolatedProcedure,
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellationToken = default)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
        try
        {
            await isolatedProcedure();
            await transaction.CommitAsync(cancellationToken);
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            await transaction.RollbackAsync(cancellationToken);
        }
    }
}
