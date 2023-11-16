using Infrastructure.IntegrationTests.Extensions;
using Microsoft.EntityFrameworkCore;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Domain.RootAggregate.Interfaces;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;
using ScienceAtrium.Domain.UserAggregate.ExecutorAggregate;
using ScienceAtrium.Domain.WorkTemplateAggregate;
using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Infrastructure.Repositories.UserAggregate;

namespace Infrastructure.IntegrationTests.DbContext;
#pragma warning disable NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method
public class ApplicationContextTests
{
    private List<string> _names;
    private IReader<Customer> _customerReader;
    private IReader<Executor> _executorReader;
    private ApplicationContext _applicationContext;

    [SetUp]
    public void Setup()
    {
        _applicationContext = new ApplicationContext(new DbContextOptionsBuilder<ApplicationContext>()
            .UseNpgsql("Server=localhost;Port=5432;Database=ScienceAtrium;User Id=postgres;Password=;Include Error Detail=true").Options);
        _customerReader = new UserRepository<Customer>(_applicationContext, null);
        _executorReader = new UserRepository<Executor>(_applicationContext, null);
        _names = new List<string>
        {
            "Nick",
            "Tom",
            "John",
            "Alex",
            "Maxim",
        };
    }
    [Test]
    public void AddNewEntityTest()
    {
        _applicationContext.Database.EnsureDeleted();
        _applicationContext.Database.Migrate();

        var order = new Order(Guid.NewGuid());

        var customer = new User(
            id: Guid.NewGuid(),
            name: TestExtension.GetRandomName(_names),
            email: TestExtension.GetRandomEmail(_names),
            phoneNumber: TestExtension.GetRandomPhoneNumber(),
            userType: UserType.Customer)
                .UpdateCurrentOrder(order).MapTo<Customer>();

        var executor = new User(
            id: Guid.NewGuid(),
            name: TestExtension.GetRandomName(_names),
            email: TestExtension.GetRandomEmail(_names),
            phoneNumber: TestExtension.GetRandomPhoneNumber(),
            userType: UserType.Executor)
                .UpdateCurrentOrder(order).MapTo<Executor>();


        var subject = new Subject(Guid.NewGuid())
        {
            Name = "Math"
        };

        order.AddWorkTemplate(new WorkTemplate(
            id: new Guid("{40405132-7516-4731-86E3-B6D4A6D956E7}"),
            title: $"{TestExtension.GetRandomEmail(_names)}-title",
            description: $"{TestExtension.GetRandomEmail(_names)}-description",
            workType: WorkType.CourseWork,
            price: Random.Shared.Next(1000, 10_000)
        )
            .UpdateSubject(subject))
            .UpdateCustomer(_customerReader, customer)
            .UpdateExecutor(_executorReader, executor);

        _applicationContext.Orders.Add(order);
        Assert.That(_applicationContext.SaveChanges(), Is.AtLeast(1));
    }

}
