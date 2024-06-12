using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Domain.WorkTemplateAggregate;
using System.Reflection;

namespace ScienceAtrium.Infrastructure.Data;

public class ApplicationContext : DbContext
{
    private static bool _isInitialized = false;
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
        if (!_isInitialized)
        {
            Database.EnsureCreated();
            _isInitialized = true;
        }
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<WorkTemplate> WorkTemplates { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<OrderWorkTemplate> OrderWorkTemplates { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Warning);
		base.OnConfiguring(optionsBuilder);
	}

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

	/// <summary>
	/// Method updates states of entity. You should use this method with method <see cref="AvoidChanges"/> for the best state tracking of entities
	/// </summary>
	/// <param name="item">entity whose state will be changed</param>
	/// <param name="state">future state of <see cref="item"/></param>
	/// <typeparam name="TEntity">type of <see cref="item"/></typeparam>
	public void TrackEntity<TEntity>(TEntity item, EntityState state)
	{
		if (item is not null)
			Entry(item).State = state;
	}

	/// <summary>
	/// Method updates states of entities. You should use this method with method <see cref="AvoidChanges"/> for the best state tracking of entities
	/// </summary>
	/// <param name="items">array of entities whose state will be changed</param>
	/// <param name="state">future state of <see cref="items"/></param>
	public void TrackEntities<TEntity>(TEntity?[] items, EntityState state)
	{
		foreach (var item in items.Where(x => x is not null))
			TrackEntity(item, state);
	}
}
