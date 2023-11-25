using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Infrastructure.Extensions;
using System.Text;
using ScienceAtrium.Domain.RootAggregate;

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

    public static string GetRandomSubject(List<string> subjects)
    {
        return subjects[Random.Shared.Next(0, subjects.Count - 1)];
    }

    public static void PrepareTests<TTrackedEntity, TUntrackedEntity>(
        ApplicationContext applicationContext,
        TTrackedEntity[] trackedEntities,
        TUntrackedEntity[]? untrackedEntities = null,
        bool ensureDeleted = true,
        bool ensureCreated = true)
        where TTrackedEntity : Entity
        where TUntrackedEntity : Entity
    {
        if (ensureDeleted)
            applicationContext.Database.EnsureDeleted();
        if (ensureCreated)
            applicationContext.Database.EnsureCreated();

        applicationContext.Set<TTrackedEntity>().AddRange(trackedEntities);
        if (untrackedEntities is not null)
            applicationContext.Set<TUntrackedEntity>().AttachRange(untrackedEntities);
        applicationContext.TrySaveChanges(null);
    }
}
