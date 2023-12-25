using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScienceAtrium.Domain.OrderAggregate;

namespace ScienceAtrium.Infrastructure.Data.Configurations.OrderAggregate;
public class OrderWorkTemplateConfiguration : IEntityTypeConfiguration<OrderWorkTemplate>
{
	public void Configure(EntityTypeBuilder<OrderWorkTemplate> builder)
	{
		builder.HasKey(x => new { x.OrderId, x.WorkTemplateId });
	}
}
