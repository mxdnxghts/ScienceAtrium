using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;
using ScienceAtrium.Domain.UserAggregate.ExecutorAggregate;
using ScienceAtrium.Domain.WorkTemplateAggregate;
using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Infrastructure.Repositories.OrderAggregate;
using ScienceAtrium.Infrastructure.Repositories.UserAggregate;
using ScienceAtrium.Infrastructure.Repositories.WorkTemplateAggregate;
using Serilog;

namespace ScienceAtrium.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDbContext<ApplicationContext>(o
            => o.UseNpgsql(configuration.GetConnectionString("ScienceAtrium_Ordering")));

        serviceCollection.AddSerilog(o => o.MinimumLevel.Warning().WriteTo.Console());

        serviceCollection.AddScoped<IOrderRepository<Order>, OrderRepository>();
        serviceCollection.AddScoped<IUserRepository<Customer>, UserRepository<Customer>>();
        serviceCollection.AddScoped<IUserRepository<Executor>, UserRepository<Executor>>();
        serviceCollection.AddScoped<IWorkTemplateRepository<WorkTemplate>, WorkTemplateRepository>();

        return serviceCollection;
    }
}
