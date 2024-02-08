using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScienceAtrium.Domain.Constants;
using ScienceAtrium.Domain.UserAggregate;

namespace ScienceAtrium.Infrastructure.UserAggregate;
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder
            .Property(x => x.Id)
            .IsRequired();
        builder
            .Property(x => x.Name)
            .HasMaxLength(EntityConfiguration.MaxLength)
            .HasField("_name")
            .IsRequired();
        builder
            .Property(x => x.Email)
            .HasMaxLength(EntityConfiguration.MaxLength)
            .HasField("_email")
            .IsRequired();
        builder
            .Property(x => x.PhoneNumber)
            .HasMaxLength(EntityConfiguration.PhoneNumberLength)
            .HasField("_phoneNumber")
            .IsRequired();
        builder
            .Property(x => x.UserType)
            .HasField("_userType")
            .IsRequired();
    }
}
