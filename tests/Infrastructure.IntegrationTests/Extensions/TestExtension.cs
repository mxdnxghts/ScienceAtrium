using ScienceAtrium.Domain.Entities;
using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Infrastructure.Extensions;
using System.Text;

namespace Infrastructure.IntegrationTests.Extensions;

public static class TestExtension
{
    public static string GetRandomName(List<string> names)
    {
        return names[Random.Shared.Next(0, names.Count - 1)];
    }

    public static string GetRandomPhoneNumber()
    {
        var sb = new StringBuilder();
        sb.Append("79");
        sb.Append(Random.Shared.Next(10, 99));
        sb.Append(Random.Shared.Next(100, 999));
        sb.Append(Random.Shared.Next(10, 99));
        sb.Append(Random.Shared.Next(10, 99));
        return sb.ToString();
    }

    public static string GetRandomEmail(List<string> names)
    {
        return $"{GetRandomName(names)}{Random.Shared.Next(10_000, 1_000_000)}@gmail.com";
    }

    public static void PrepareTests<TEntity>(ApplicationContext applicationContext, TEntity[] entities)
        where TEntity : Entity
    {
        applicationContext.Database.EnsureDeleted();
        applicationContext.Database.EnsureCreated();

        applicationContext.Set<TEntity>().AddRange(entities);
        applicationContext.TrySaveChanges(null);
    }
}
