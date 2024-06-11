using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Domain.RootAggregate.Interfaces;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;
using ScienceAtrium.Domain.UserAggregate.ExecutorAggregate;
using ScienceAtrium.Infrastructure.Constants;
using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Infrastructure.OrderAggregate;
using ScienceAtrium.Infrastructure.UserAggregate;
using Serilog;

namespace ScienceAtrium.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
#if DEBUG
        serviceCollection.AddDbContext<ApplicationContext>(o
            => o.UseNpgsql(configuration.GetConnectionString("ScienceAtriumOrder")));

        serviceCollection.AddDbContext<IdentityContext>(o
            => o.UseNpgsql(configuration.GetConnectionString("ScienceAtriumOrder")));
#else

        serviceCollection.AddDbContext<ApplicationContext>(o
            => o.UseNpgsql(configuration.GetConnectionString(ConnectionConfigurationConstants.ProductionConnectionString)));

        serviceCollection.AddDbContext<IdentityContext>(o
               => o.UseNpgsql(configuration.GetConnectionString(ConnectionConfigurationConstants.ProductionConnectionString)));
#endif

        serviceCollection.AddSerilog(o =>
        {
            o.MinimumLevel.Warning().WriteTo.Console()
                .WriteTo.File(configuration.GetRequiredSection("Logging:Path:SerilogInfo").Value);

            o.MinimumLevel.Information().WriteTo.File(configuration.GetRequiredSection("Logging:Path:SerilogInfo").Value);
            o.MinimumLevel.Error().WriteTo.File(configuration.GetRequiredSection("Logging:Path:SerilogError").Value);
        });

        serviceCollection.AddStackExchangeRedisCache(options =>
        {
            options.InstanceName = "ScienceAtriumCache_";
#if DEBUG
            options.Configuration = configuration.GetConnectionString(ConnectionConfigurationConstants.DevelopmentConnectionStringRedis);
#else
            options.Configuration = configuration.GetConnectionString(ConnectionConfigurationConstants.ProductionConnectionStringRedis);
#endif
		});

        serviceCollection.AddScoped<IOrderRepository<Order>, OrderRepository>();
        serviceCollection.AddScoped<IUserRepository<Customer>, UserRepository<Customer>>();
        serviceCollection.AddScoped<IUserRepository<Executor>, UserRepository<Executor>>();

        serviceCollection.AddReaders();

        serviceCollection.AddTransient<ApplicationTransactionService>();

        return serviceCollection;
    }

    private static void AddReaders(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IReader<Customer>, UserRepository<Customer>>();

        // Used transient lifetime due to it is used in UserAuthorizationHandler
        serviceCollection.AddScoped<IReaderAsync<Customer>, UserRepository<Customer>>();

        serviceCollection.AddScoped<IReader<Executor>, UserRepository<Executor>>();
        serviceCollection.AddScoped<IReaderAsync<Executor>, UserRepository<Executor>>();
    }
}
