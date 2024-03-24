using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Domain.RootAggregate.Interfaces;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;
using ScienceAtrium.Domain.UserAggregate.ExecutorAggregate;
using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Infrastructure.OrderAggregate;
using ScienceAtrium.Infrastructure.UserAggregate;
using Serilog;

namespace ScienceAtrium.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        //      serviceCollection.AddDbContext<ApplicationContext>(o
        //          => o.UseSqlServer(configuration.GetConnectionString("MSSQL")));

        //serviceCollection.AddDbContext<IdentityContext>(o
        //	=> o.UseSqlServer(configuration.GetConnectionString("MSSQL")));

        serviceCollection.AddDbContext<ApplicationContext>(o
            => o.UseNpgsql(configuration.GetConnectionString("ScienceAtriumRelease")));

        serviceCollection.AddDbContext<IdentityContext>(o
               => o.UseNpgsql(configuration.GetConnectionString("ScienceAtriumRelease")));


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
            options.Configuration = configuration.GetConnectionString("ScienceAtriumRedisCacheRelease");
            //options.Configuration = configuration.GetConnectionString("ScienceAtriumRedisCache");
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
        serviceCollection.AddScoped<IReaderAsync<Customer>, UserRepository<Customer>>();

        serviceCollection.AddScoped<IReader<Executor>, UserRepository<Executor>>();
        serviceCollection.AddScoped<IReaderAsync<Executor>, UserRepository<Executor>>();
    }
}
