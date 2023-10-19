using Infrastructure.IntegrationTests.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestPlatform.CoreUtilities.Extensions;
using ScienceAtrium.Domain.Entities;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Domain.WorkTemplateAggregate;
using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Infrastructure.Repositories.OrderAggregation;
using ScienceAtrium.Infrastructure.Repositories.WorkTemplateAggregation;
using System.Globalization;

namespace Infrastructure.IntegrationTests.Repositories.WorkTemplateAggregate;

public class WorkTemplateRepositoryTest
{
    private IWorkTemplateRepository<WorkTemplate> _workTemplateRepository;
    private ApplicationContext _applicationContext;
    private List<string> _names;
    private List<string> _subjects;

    [SetUp]
    public void Setup()
    {
        _applicationContext = new ApplicationContext(
            new DbContextOptionsBuilder<ApplicationContext>()
            .UseNpgsql("Server=localhost;Port=5432;Database=ScienceAtrium;User Id=postgres;Password=;Include Error Detail=true").Options);

        _workTemplateRepository = new WorkTemplateRepository(_applicationContext, null);
        _names = new List<string>
        {
            "Nick",
            "Tom",
            "John",
            "Alex",
            "Maxim",
        };

        _subjects = new List<string>
        {
            "Math",
            "PE",
            "Art",
            "English",
            "History",
            "Music",
            "Science",
        };

        _applicationContext.Database.EnsureCreated();
    }

    [Test]
    public void PrepareTests()
    {
        var subjects = GetSubjectEntities().ToArray();
        TestExtension.PrepareTests<Subject, Entity>(_applicationContext,
            subjects, ensureDeleted: true);

        TestExtension.PrepareTests(_applicationContext,
            GetWorkTemplateEntities(100), subjects, ensureDeleted: false);

        Assert.Pass();
    }

    [Test]
    public void CreateWorkTemplateTest()
    {
        var wt = GetWorkTemplateEntity();

        _workTemplateRepository.Create(wt);

        Assert.That(_workTemplateRepository.Get(x => x.Id == wt.Id),
            Is.Not.EqualTo(WorkTemplate.Default));
    }

    [Test]
    public async Task CreateWorkTemplateAsyncTest()
    {
        var wt = GetWorkTemplateEntity();

        await _workTemplateRepository.CreateAsync(wt);

        Assert.That(await _workTemplateRepository.GetAsync(x => x.Id == wt.Id),
            Is.Not.EqualTo(WorkTemplate.Default));
    }

    [Test]
    public void DeleteWorkTemplateTest()
    {
        var wt = _workTemplateRepository.Get(x => x.Price != 0);

        _workTemplateRepository.Delete(wt);

        Assert.That(_workTemplateRepository.Get(x => x.Id == wt.Id),
            Is.EqualTo(WorkTemplate.Default));
    }

    [Test]
    public async Task DeleteWorkTemplateAsyncTest()
    {
        var wt = await _workTemplateRepository.GetAsync(x => x.Price != 0);

        await _workTemplateRepository.DeleteAsync(wt);

        Assert.That(await _workTemplateRepository.GetAsync(x => x.Id == wt.Id),
            Is.EqualTo(WorkTemplate.Default));
    }

    [Test]
    public void UpdateWorkTemplateTest()
    {
        var wt = _workTemplateRepository.Get(x => x.Price != 0);
        var oldWt = _workTemplateRepository.Get(x => x.Price != 0);
        wt.Description = "new";

        _workTemplateRepository.Update(wt);

        Assert.That(_workTemplateRepository.Get(x => x.Id == wt.Id).Description,
            Is.Not.EqualTo(oldWt.Description));
    }

    [Test]
    public async Task UpdateWorkTemplateAsyncTest()
    {
        var wt = await _workTemplateRepository.GetAsync(x => x.Price != 0);
        var oldWt = _workTemplateRepository.Get(x => x.Price != 0);
        wt.Description = "new";

        await _workTemplateRepository.UpdateAsync(wt);

        Assert.That(_workTemplateRepository.Get(x => x.Id == wt.Id).Description,
            Is.Not.EqualTo(oldWt.Description));
    }

    [Test]
    public void GetWorkTemplateTest()
    {
        var workTemplate = _workTemplateRepository.Get(x =>
            x.Id != Guid.Empty);

        Assert.That(workTemplate,
            Is.Not.EqualTo(WorkTemplate.Default));
    }

    [Test]
    public async Task GetWorkTemplateAsyncTest()
    {
        var workTemplate = await _workTemplateRepository.GetAsync(x =>
            x.Id != Guid.Empty);

        Assert.That(workTemplate,
            Is.Not.EqualTo(WorkTemplate.Default));
    }

    private WorkTemplate[] GetWorkTemplateEntities(int count)
    {
        var workTemplates = new WorkTemplate[count];
        for (int i = 0; i < count; i++)
            workTemplates[i] = GetWorkTemplateEntity();
        return workTemplates;
    }

    private IEnumerable<Subject> GetSubjectEntities()
    {
        foreach (var subject in _subjects)
        {
            yield return new Subject(Guid.NewGuid())
            {
                Name = subject,
            };
        }
    }

    private WorkTemplate GetWorkTemplateEntity()
    {
        var subject = _applicationContext.Subjects.SingleOrDefault(x => x.Name == TestExtension.GetRandomSubject(_subjects));

        return new WorkTemplate(Guid.NewGuid())
        {
            Title = $"{TestExtension.GetRandomEmail(_names)}-title",
            Description = $"{TestExtension.GetRandomEmail(_names)}-description",
            WorkType = WorkType.CourseWork,
            Price = Random.Shared.Next(1000, 10_000),
            Subject = subject,
            SubjectId = subject.Id,
        };
    }
}
