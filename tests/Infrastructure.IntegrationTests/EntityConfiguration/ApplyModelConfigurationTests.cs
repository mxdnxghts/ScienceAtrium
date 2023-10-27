using Microsoft.EntityFrameworkCore;
using ScienceAtrium.Infrastructure.Data;

namespace Infrastructure.IntegrationTests.EntityConfiguration;

public class ApplyModelConfigurationTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void ApplyConfigurationsTest()
    {
        var context = new ApplicationContext(new DbContextOptionsBuilder<ApplicationContext>()
            .UseNpgsql("Server=localhost;Port=5432;Database=ScienceAtrium;User Id=postgres;Password=").Options);

        context.Database.EnsureDeleted();

        Assert.IsTrue(context.Database.EnsureCreated());
    }
}