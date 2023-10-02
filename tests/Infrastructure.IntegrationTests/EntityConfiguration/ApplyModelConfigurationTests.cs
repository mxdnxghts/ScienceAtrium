using Microsoft.EntityFrameworkCore;
using ScienceAtrium.Infrastructure.Data;

namespace Infrastructure.IntegrationTests.EntityConfiguration;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void ApplyMigrationsTest()
    {
        var context = new ApplicationContext(new DbContextOptionsBuilder<ApplicationContext>()
            .UseNpgsql("Server=localhost;Port=5432;Database=ScienceAtrium;User Id=;Password=").Options);

        Assert.IsTrue(context.Database.EnsureCreated());
    }
}