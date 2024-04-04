using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ScienceAtrium.Application.UserAggregate.CustomerAggregate.Commands;
using ScienceAtrium.Application.UserAggregate.CustomerAggregate.Queries;
using ScienceAtrium.Domain.RootAggregate.Options;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;
using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Presentation.UserAggregate;

namespace Microsoft.AspNetCore.Routing;

internal static class IdentityComponentsEndpointRouteBuilderExtensions
{
    // These endpoints are required by the Identity Razor components defined in the /Components/Account/Pages directory of this project.
    public static IEndpointConventionBuilder MapAdditionalIdentityEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        return endpoints.MapGet("/sign-in", async (
            HttpContext context,
            [FromServices] SignInManager<ApplicationUser> signInManager,
            [FromServices] IMediator mediator,
            [FromServices] Serilog.ILogger logger,
            [FromServices] IMapper mapper,
			[FromQuery(Name = "user_email")] string userEmail,
			[FromQuery(Name = "user_name")] string userName,
			[FromQuery(Name = "return_uri")] string returnUri) =>
		{
			var idp = DataProtectionProvider.Create(UserConstants.DataProtectionApplicationName);
			var protector = idp.CreateProtector(UserConstants.DataProtectorPurpose);

			var email = protector.Unprotect(userEmail);
            var name = protector.Unprotect(userName);
            await OnLoginCallbackAsync(mediator, logger, mapper, email, name);
            context.Response.Redirect(returnUri);
        });
    }

    private static async Task OnLoginCallbackAsync(
        IMediator mediator,
        Serilog.ILogger logger,
        IMapper mapper,
        string email,
        string name)
    {
		var customer = await mediator.Send(
            new GetCustomerQuery(
                new EntityFindOptions<Customer>(predicate: x => x.Email == email)));
        if (!customer.IsEmpty())
        {
            logger.Warning("Customer with Id {0} has logged in.", customer.Id);
            return;
        }

        var user = new Customer(Guid.NewGuid())
            .UpdateName(name)
            .UpdateEmail(email)
            .UpdatePhoneNumber("null");

        customer = mapper.Map<Customer>(user);

        var customerCreationResult = await mediator.Send(new CreateCustomerCommand(customer));
        if (customerCreationResult <= 0)
        {
            logger.Error("Didn't create new customer with Id {0}. Count of changes {1}", customer.Id, customerCreationResult);
            return;
        }

        await mediator.Send(new UpdateCachedCustomerCommand(customer));

        logger.Information("Created new customer with Id {0}", customer.Id);
    }
}