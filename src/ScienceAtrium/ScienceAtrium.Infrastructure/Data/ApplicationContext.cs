using Microsoft.EntityFrameworkCore;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Domain.WorkTemplateAggregate;
using System.Reflection;

namespace ScienceAtrium.Infrastructure.Data;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<WorkTemplate> WorkTemplates { get; set; }
    public DbSet<Subject> Subjects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges()
    {
        var changes = base.SaveChanges();
        ChangeTracker.Clear();
        return changes;
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        var changes = base.SaveChanges(acceptAllChangesOnSuccess);
        ChangeTracker.Clear();
        return changes;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var changes = await base.SaveChangesAsync(cancellationToken);
        ChangeTracker.Clear();
        return changes;
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        var changes = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        ChangeTracker.Clear();
        return changes;
    }
}
