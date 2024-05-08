using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScienceAtrium.Application;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Domain.RootAggregate.Interfaces;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;
using ScienceAtrium.Domain.UserAggregate.ExecutorAggregate;
using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Infrastructure.OrderAggregate;
using ScienceAtrium.Infrastructure.UserAggregate;
using Serilog;

namespace Infrastructure.IntegrationTests;
public class Setup
{
	private const string SqlServerConnectionString = "Server=localhost\\\\SQLEXPRESS;Data Source=maxim;Initial Catalog=Test;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False; Encrypt=True;TrustServerCertificate=True";
	private const string RedisConnectionString = "localhost:6379";
	private const string LogPath = "../../../test_logs.log";

	private Setup() { }

	public static IServiceProvider Provider { get; private set; }
	public static List<string> Names { get; private set; }

	private static ServiceCollection Services
	{
		get
		{
			var services = new ServiceCollection();

			services.AddSerilog(o =>
			{
				o.MinimumLevel.Warning()
					.WriteTo.Console()
					.WriteTo.File(LogPath);
			});
			services.AddDbContext<ApplicationContext>(o => o.UseSqlServer(SqlServerConnectionString));
			services.AddStackExchangeRedisCache(options =>
			{
				options.InstanceName = "ScienceAtriumCache_";
				options.Configuration = RedisConnectionString;
			});
			services.AddScoped<IOrderRepository<Order>, OrderRepository>();
			services.AddScoped<IUserRepository<Customer>, UserRepository<Customer>>();
			services.AddScoped<IUserRepository<Executor>, UserRepository<Executor>>();

			services.AddScoped<IReader<Customer>, UserRepository<Customer>>();

			// Used transient lifetime due to it is used in UserAuthorizationHandler
			services.AddScoped<IReaderAsync<Customer>, UserRepository<Customer>>();

			services.AddScoped<IReader<Executor>, UserRepository<Executor>>();
			services.AddScoped<IReaderAsync<Executor>, UserRepository<Executor>>();

			services.AddTransient<ApplicationTransactionService>();

			services.AddApplication();

			return services;
		}
	}

	public static void StartUp()
	{
		PrepareLogFile(LogPath);
		Provider = Services.BuildServiceProvider();

		Names = new List<string>
		{
			"Nick",
			"Tom",
			"John",
			"Alex",
			"Maxim",
		};
	}

	public static void TurnOff()
	{
		Services.Clear();
		Names = null;
	}

	private static void PrepareLogFile(string path)
	{
		File.WriteAllText(path, $"{DateTime.Now}\n\n", Encoding.UTF8);
	}
}
