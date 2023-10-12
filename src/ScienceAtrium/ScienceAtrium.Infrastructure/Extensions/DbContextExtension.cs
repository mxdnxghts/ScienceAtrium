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
            transaction?.Commit();
            return changes;
        }
        catch (Exception e) when (e is DbUpdateConcurrencyException or DbUpdateException)
        {
            logger?.Error(e, e.Message);
            await transaction?.RollbackAsync();
        }
        catch (Exception e)
        {
            await transaction?.RollbackAsync();
            throw e;
        }
        return -1;
    }
}
