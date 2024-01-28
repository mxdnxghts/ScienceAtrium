using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;

namespace ScienceAtrium.Infrastructure.Extensions;

/// <summary>
/// This extension class provides a convenient way to handle exceptions that may occur during operate with data
/// </summary>
public static class DbContextExtension
{
    public static int TrySaveChanges(
        this DbContext context,
        ILogger? logger,
        IDbContextTransaction? transaction = null)
    {
        try
        {
            var changes = context.SaveChanges();
            transaction?.Commit();
			context.ChangeTracker.Clear();
			return changes;
        }
        catch (Exception e) when (e is DbUpdateConcurrencyException or DbUpdateException)
        {
            logger?.Error(e, e.Message);
            transaction?.Rollback();
        }
        catch (Exception e)
        {
            transaction?.Rollback();
            throw e;
        }
        return -1;
    }

    public static async Task<int> TrySaveChangesAsync(
        this DbContext context,
        ILogger? logger,
        IDbContextTransaction? transaction = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var changes = await context.SaveChangesAsync(cancellationToken);
            if (transaction is not null)
                await transaction.CommitAsync(cancellationToken);
            context.ChangeTracker.Clear();
            return changes;
        }
        catch (Exception e) when (e is DbUpdateConcurrencyException or DbUpdateException)
        {
            logger?.Error(e, e.Message);
            if (transaction is not null)
                await transaction.RollbackAsync(cancellationToken);
        }
        catch (Exception e)
        {
            if (transaction is not null)
                await transaction.RollbackAsync(cancellationToken);
            throw e;
        }
        return -1;
    }
}
