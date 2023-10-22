using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScienceAtrium.Domain.UserAggregate.ExecutorAggregate;

namespace ScienceAtrium.Infrastructure.Data.Configurations.UserAggregate;
public class ExecutorConfiguration : IEntityTypeConfiguration<Executor>
{
    public void Configure(EntityTypeBuilder<Executor> builder)
    {
        builder
            .HasMany(x => x.DoneOrders)
            .WithOne()
            .OnDelete(DeleteBehavior.NoAction);
    }
}
