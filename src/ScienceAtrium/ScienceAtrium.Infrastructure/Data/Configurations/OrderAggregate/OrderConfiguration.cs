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

        builder.Property(x => x.TotalCost)
            .HasPrecision(12, 2)
            .HasField("_totalCost")
            .IsRequired();
        builder
            .Property(x => x.OrderDate)
            .IsRequired();
        builder
            .Property(x => x.PaymentMethod)
            .HasField("_paymentMethod")
            .IsRequired();
        builder
            .Property(x => x.Status)
            .HasField("_status")
            .IsRequired();
    }
}
