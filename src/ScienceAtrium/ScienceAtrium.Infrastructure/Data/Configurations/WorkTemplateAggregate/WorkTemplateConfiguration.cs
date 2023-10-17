using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScienceAtrium.Domain.Constants;
using ScienceAtrium.Domain.WorkTemplateAggregate;

namespace ScienceAtrium.Infrastructure.Data.Configurations.WorkTemplateAggregate;
public class WorkTemplateConfiguration : IEntityTypeConfiguration<WorkTemplate>
{
    public void Configure(EntityTypeBuilder<WorkTemplate> builder)
    {
        builder.HasKey(x => x.Id);
        builder
            .HasOne(x => x.Subject)
            .WithOne()
            .HasForeignKey<WorkTemplate>(x => x.SubjectId)
            .OnDelete(DeleteBehavior.ClientSetNull);
        builder
            .Property(x => x.Title)
            .HasMaxLength(EntityConfiguration.MaxLength)
            .IsRequired();

        builder
            .Property(x => x.Description)
            .HasMaxLength(EntityConfiguration.MaxLength);

        builder
            .Property(x => x.WorkType)
            .IsRequired();

        builder
            .Property(x => x.Price)
            .HasPrecision(12, 2)
            .IsRequired();
    }
}
