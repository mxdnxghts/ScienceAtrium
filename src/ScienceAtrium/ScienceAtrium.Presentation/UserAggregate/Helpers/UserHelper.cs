using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using ScienceAtrium.Application.UserAggregate.CustomerAggregate.Queries;
using ScienceAtrium.Domain.RootAggregate.Options;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;
using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Presentation.UserAggregate.Constants;

namespace ScienceAtrium.Presentation.UserAggregate.Helpers;

public static class UserHelper
{
    public static async Task<Customer> GetUser(IMediator mediator,
                                               string? protectedId,
                                               string? protectedEmail,
                                               bool forceDatabaseSearch = true)
    {
        var userId = GetUnprotectedUserId(protectedId);
        var userEmail = GetUnprotectedUserEmail(protectedEmail);
        EntityFindOptions<Customer> options;
        if (!userId.Equals(Guid.Empty))
        {
            options = new EntityFindOptions<Customer>(userId);
        }
        else
        {
            options = new EntityFindOptions<Customer>(predicate: c => c.Email == userEmail);
        }

        if (forceDatabaseSearch)
            options.ForceDatabaseSearch();

        return await mediator.Send(new GetCustomerQuery(options));
    }

    public static async Task<string> GetUserTypeAsync(string email, string connectionString)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(connectionString))
            return string.Empty;
#if DEBUG
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseSqlServer(connectionString)
            .Options;
#else
        var options = new DbContextOptionsBuilder<ApplicationContext>()
			.UseNpgsql(connectionString)
			.Options;
#endif

		await using var context = new ApplicationContext(options);

        var userType = (await context.Users.FirstOrDefaultAsync(u => u.Email == email))?.UserType
            ?? UserType.Customer;

        if (userType.Equals(UserType.Customer))
            return UserAuthorizationConstants.CustomerRole;
        else if (userType.Equals(UserType.Executor))
            return UserAuthorizationConstants.ExecutorRole;
        else
            return string.Empty;

	}

    internal static Guid GetUnprotectedUserId(string? protectedId)
    {
        if (protectedId is null or "")
            return Guid.Empty;
        var protector = DataProtectionProvider.Create(UserConstants.DataProtectionApplicationName)
            .CreateProtector(UserConstants.DataProtectorPurpose);
        var unprotectedUserId = protector.Unprotect(protectedId);
        if (unprotectedUserId == "")
            return Guid.Empty;
        return Guid.Parse(unprotectedUserId);
    }

    internal static string GetUnprotectedUserEmail(string? protectedEmail)
    {
        if (protectedEmail is null or "")
            return string.Empty;
        var protector = DataProtectionProvider.Create(UserConstants.DataProtectionApplicationName)
            .CreateProtector(UserConstants.DataProtectorPurpose);
        var unprotectedUserEmail = protector.Unprotect(protectedEmail);
        return unprotectedUserEmail;
    }
}
