using Microsoft.Extensions.DependencyInjection;

namespace ScienceAtrium.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMediatR(c =>
        {
            c.RegisterServicesFromAssembly(typeof(MediatREntryPoint).Assembly);
        });
        return serviceCollection;
    }
}
