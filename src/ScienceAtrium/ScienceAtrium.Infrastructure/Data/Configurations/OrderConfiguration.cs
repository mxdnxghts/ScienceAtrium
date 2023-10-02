using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScienceAtrium.Infrastructure.Data.Configurations;
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(x => x.Id);
        builder
            .HasOne(x => x.Customer)
            .WithOne()
            .HasForeignKey<Order>(x => x.CustomerId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasOne(x => x.Executor)
            .WithOne()
            .HasForeignKey<Order>(x => x.ExecutorId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasMany(x => x.WorkTemplates)
            .WithOne()
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .Property(x => x.TotalPrice).
            IsRequired();
        builder
            .Property(x => x.OrderDate)
            .HasPrecision(12, 2)
            .IsRequired();
        builder
            .Property(x => x.PaymentMethod)
            .IsRequired();
        builder
            .Property(x => x.Status)
            .IsRequired();
    }
}
