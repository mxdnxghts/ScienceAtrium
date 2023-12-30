using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;
using ScienceAtrium.Domain.UserAggregate.ExecutorAggregate;

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
            mc.AddUserMapper();
		});
        return serviceCollection;
    }

    private static IMapperConfigurationExpression AddUserMapper(this IMapperConfigurationExpression mapperConfiguration)
    {
#pragma warning disable CS8603 // Possible null reference return.
        mapperConfiguration.CreateMap<CustomerJson, Customer>().ConstructUsing(customerJson =>
            new Customer(customerJson.Id)
            .UpdateName(customerJson.Name)
            .UpdateEmail(customerJson.Email)
            .UpdatePhoneNumber(customerJson.PhoneNumber)
            .UpdateUserType(customerJson.UserType) as Customer);
        mapperConfiguration.CreateMap<User, Customer>().ConstructUsing(user =>
            new Customer(user.Id)
            .UpdateName(user.Name)
            .UpdateEmail(user.Email)
            .UpdatePhoneNumber(user.PhoneNumber)
            .UpdateUserType(user.UserType) as Customer);

        mapperConfiguration.CreateMap<ExecutorJson, Executor>().ConstructUsing(customerJson =>
            new Executor(customerJson.Id)
            .UpdateName(customerJson.Name)
            .UpdateEmail(customerJson.Email)
            .UpdatePhoneNumber(customerJson.PhoneNumber)
            .UpdateUserType(customerJson.UserType) as Executor);
        mapperConfiguration.CreateMap<User, Executor>().ConstructUsing(user =>
            new Executor(user.Id)
            .UpdateName(user.Name)
            .UpdateEmail(user.Email)
            .UpdatePhoneNumber(user.PhoneNumber)
            .UpdateUserType(user.UserType) as Executor);
#pragma warning restore CS8603 // Possible null reference return.
        return mapperConfiguration;
    }

}
