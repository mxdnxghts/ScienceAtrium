using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScienceAtrium.Domain.OrderAggregate;

namespace ScienceAtrium.Infrastructure.Data.Configurations.OrderAggregate;
public class BasketConfiguration : IEntityTypeConfiguration<Basket>
{
    public void Configure(EntityTypeBuilder<Basket> builder)
    {
        builder.HasOne(x => x.Order).WithOne().HasForeignKey<Basket>(x => x.OrderId);
        builder.HasOne(x => x.Customer).WithOne().HasForeignKey<Basket>(x => x.Customerid);
    }
}
