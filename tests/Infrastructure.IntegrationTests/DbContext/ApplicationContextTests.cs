using Infrastructure.IntegrationTests.Extensions;
using Microsoft.EntityFrameworkCore;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Domain.RootAggregate.Interfaces;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Domain.WorkTemplateAggregate;
using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Infrastructure.UserAggregate;

namespace Infrastructure.IntegrationTests.DbContext;
#pragma warning disable NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method
public class ApplicationContextTests
{
    private List<string> _names;
    private IReader<User> _userReader;
    private ApplicationContext _applicationContext;

    [SetUp]
    public void Setup()
    {
        _applicationContext = new ApplicationContext(new DbContextOptionsBuilder<ApplicationContext>()
            .UseNpgsql("Server=localhost;Port=5432;Database=ScienceAtrium;User Id=postgres;Password=;Include Error Detail=true").Options);
        _userReader = new UserRepository<User>(_applicationContext, null, null);
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
                .AddOrder(order);

        var executor = new User(
            id: Guid.NewGuid(),
            name: TestExtension.GetRandomName(_names),
            email: TestExtension.GetRandomEmail(_names),
            phoneNumber: TestExtension.GetRandomPhoneNumber(),
            userType: UserType.Executor)
                .AddOrder(order);


        var subject = new Subject(Guid.NewGuid())
        {
            Name = "Math"
        };

        //order.AddWorkTemplate(new WorkTemplate(
        //    id: new Guid("{40405132-7516-4731-86E3-B6D4A6D956E7}"),
        //    title: $"{TestExtension.GetRandomEmail(_names)}-title",
        //    description: $"{TestExtension.GetRandomEmail(_names)}-description",
        //    workType: WorkType.CourseWork,
        //    price: Random.Shared.Next(1000, 10_000)
        //)
        order.AddWorkTemplate(new WorkTemplate(Guid.NewGuid())
            .UpdateSubject(subject));
            //.UpdateCustomer(_userReader, customer)
            //.UpdateExecutor(_userReader, executor);

        _applicationContext.Orders.Add(order);
        Assert.That(_applicationContext.SaveChanges(), Is.AtLeast(1));
    }

}
