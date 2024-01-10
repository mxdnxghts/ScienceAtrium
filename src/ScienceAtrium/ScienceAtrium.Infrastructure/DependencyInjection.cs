using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Domain.RootAggregate.Interfaces;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;
using ScienceAtrium.Domain.UserAggregate.ExecutorAggregate;
using ScienceAtrium.Domain.WorkTemplateAggregate;
using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Infrastructure.OrderAggregate;
using ScienceAtrium.Infrastructure.UserAggregate;
using ScienceAtrium.Infrastructure.WorkTemplateAggregate;
using Serilog;

namespace ScienceAtrium.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDbContext<ApplicationContext>(o
            => o.UseSqlServer(configuration.GetConnectionString("MSSQL")));

        serviceCollection.AddSerilog(o =>
        {
            o.MinimumLevel.Warning().WriteTo.Console();
            o.MinimumLevel.Warning().WriteTo.File(configuration.GetRequiredSection("Logging:Path:Serilog").Value);
        });

        serviceCollection.AddScoped<IOrderRepository<Order>, OrderRepository>();
        serviceCollection.AddScoped<IUserRepository<Customer>, UserRepository<Customer>>();
        serviceCollection.AddScoped<IUserRepository<Executor>, UserRepository<Executor>>();
        serviceCollection.AddScoped<IWorkTemplateRepository<WorkTemplate>, WorkTemplateRepository>();

        serviceCollection.AddReaders();

        serviceCollection.AddStackExchangeRedisCache(options =>
        {
            options.InstanceName = "ScienceAtriumCache_";
            options.Configuration = configuration.GetConnectionString("ScienceAtriumRedisCache");
        });

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
