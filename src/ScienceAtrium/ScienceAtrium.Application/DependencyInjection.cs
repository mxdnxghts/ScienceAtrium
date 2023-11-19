using Microsoft.Extensions.DependencyInjection;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;
using ScienceAtrium.Domain.UserAggregate.ExecutorAggregate;
using ScienceAtrium.Domain.UserAggregate;

namespace ScienceAtrium.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMediatR(c => c.RegisterServicesFromAssembly(typeof(MediatREntryPoint).Assembly));
        serviceCollection.AddAutoMapper(mc =>
        {
            mc.CreateMap<User, Customer>();
            mc.CreateMap<User, Executor>();

#pragma warning disable CS8603 // Possible null reference return.
			mc.CreateMap<CustomerJson, Customer>().ConstructUsing(customerJson =>
                new Customer(customerJson.Id)
                .UpdateName(customerJson.Name)
                .UpdateEmail(customerJson.Email)
                .UpdatePhoneNumber(customerJson.PhoneNumber)
                .UpdateCurrentOrder(customerJson.CurrentOrder)
                .UpdateUserType(customerJson.UserType) as Customer);
			mc.CreateMap<ExecutorJson, Executor>().ConstructUsing(customerJson =>
				new Executor(customerJson.Id)
				.UpdateName(customerJson.Name)
				.UpdateEmail(customerJson.Email)
				.UpdatePhoneNumber(customerJson.PhoneNumber)
				.UpdateCurrentOrder(customerJson.CurrentOrder)
				.UpdateUserType(customerJson.UserType) as Executor);
#pragma warning restore CS8603 // Possible null reference return.
		});
        return serviceCollection;
    }
}
