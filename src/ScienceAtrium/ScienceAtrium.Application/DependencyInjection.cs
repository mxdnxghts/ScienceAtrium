using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;
using ScienceAtrium.Domain.UserAggregate.ExecutorAggregate;
using System.Reflection;

namespace ScienceAtrium.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMediatR(c => c.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        serviceCollection.AddAutoMapper(mc =>
        {
            mc.CreateMap<User, Customer>();
            mc.CreateMap<User, Executor>();
		});
        return serviceCollection;
    }
}
