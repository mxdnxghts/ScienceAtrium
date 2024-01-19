using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ScienceAtrium.Infrastructure.Data;
public sealed class ApplicationTransactionService(ApplicationContext _context, Serilog.ILogger _logger)
{
    public async Task BeginTransactionAsync(
        IsolationLevel isolationLevel,
        Action isolatedProcedure,
        CancellationToken cancellationToken = default)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
        try
        {
            isolatedProcedure();
            await transaction.CommitAsync(cancellationToken);
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            await transaction.RollbackAsync(cancellationToken);
        }
    }
}
