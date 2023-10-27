using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScienceAtrium.Domain.OrderAggregate;

namespace ScienceAtrium.Infrastructure.Data.Configurations.OrderAggregate;
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(x => x.Id);
        builder
            .HasOne(x => x.Customer)
            .WithOne(x => x.CurrentOrder)
            .HasForeignKey<Order>(x => x.CustomerId)
            .OnDelete(DeleteBehavior.ClientSetNull);
        builder
            .HasOne(x => x.Executor)
            .WithOne(x => x.CurrentOrder)
            .HasForeignKey<Order>(x => x.ExecutorId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder
            .HasMany(x => x.WorkTemplates)
            .WithMany();

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
