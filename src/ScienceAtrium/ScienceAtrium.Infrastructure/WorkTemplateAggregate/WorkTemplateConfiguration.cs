using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScienceAtrium.Domain.Constants;
using ScienceAtrium.Domain.WorkTemplateAggregate;

namespace ScienceAtrium.Infrastructure.WorkTemplateAggregate;
public class WorkTemplateConfiguration : IEntityTypeConfiguration<WorkTemplate>
{
    public void Configure(EntityTypeBuilder<WorkTemplate> builder)
    {
        builder.HasKey(x => x.Id);
        builder
            .HasOne(x => x.Subject)
            .WithMany()
            .HasForeignKey(x => x.SubjectId)
            .OnDelete(DeleteBehavior.ClientSetNull);
        builder
            .Property(x => x.Title)
            .HasMaxLength(EntityConfiguration.MaxLength)
            .HasField("_title")
            .IsRequired();

        builder
            .Property(x => x.Description)
            .HasField("_description")
            .HasMaxLength(EntityConfiguration.MaxLength);

        builder
            .Property(x => x.WorkType)
            .HasField("_workType")
            .IsRequired();

        builder
            .Property(x => x.Price)
            .HasPrecision(12, 2)
            .HasField("_price")
            .IsRequired();
    }
}
