using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScienceAtrium.Domain.Constants;
using ScienceAtrium.Domain.UserAggregate;

namespace ScienceAtrium.Infrastructure.Data.Configurations;
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(EntityConfiguration.MaxLength);
        builder.Property(x => x.PhoneNumber).HasMaxLength(EntityConfiguration.PhoneNumberLength);
    }
}
