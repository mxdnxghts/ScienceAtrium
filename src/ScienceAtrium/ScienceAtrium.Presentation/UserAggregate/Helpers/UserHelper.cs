using MediatR;
using Microsoft.AspNetCore.DataProtection;
using ScienceAtrium.Application.UserAggregate.CustomerAggregate.Queries;
using ScienceAtrium.Domain.RootAggregate.Options;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;

namespace ScienceAtrium.Presentation.UserAggregate.Helpers;

public static class UserHelper
{
    public static async Task<Customer> GetUser(IMediator mediator,
                                               string? protectedId,
                                               string? protectedEmail,
                                               bool forceDatabaseSearch = true)
    {
        var userId = GetUnprotectedCustomerId(protectedId);
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

    private static Guid GetUnprotectedCustomerId(string? protectedId)
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

    private static string GetUnprotectedUserEmail(string? protectedEmail)
    {
        if (protectedEmail is null or "")
            return string.Empty;
        var protector = DataProtectionProvider.Create(UserConstants.DataProtectionApplicationName)
            .CreateProtector(UserConstants.DataProtectorPurpose);
        var unprotectedUserEmail = protector.Unprotect(protectedEmail);
        return unprotectedUserEmail;
    }
}
