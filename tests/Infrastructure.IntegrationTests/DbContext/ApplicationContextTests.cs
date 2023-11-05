using Microsoft.EntityFrameworkCore;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;
using ScienceAtrium.Domain.WorkTemplateAggregate;
using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Domain.UserAggregate.ExecutorAggregate;

namespace Infrastructure.IntegrationTests.DbContext;

public class ApplicationContextTests
{
    [Test]
    public void AddNewEntityTest()
    {
        var context = new ApplicationContext(new DbContextOptionsBuilder<ApplicationContext>()
            .UseNpgsql("Server=localhost;Port=5432;Database=ScienceAtrium;User Id=postgres;Password=;Include Error Detail=true").Options);

        context.Database.EnsureDeleted();
        context.Database.Migrate();
        var customer = new Customer(Guid.NewGuid())
        {
            Name = "Customer",
            Email = "customer@gmail.ru",
            PhoneNumber = "123",
            UserType = UserType.Customer,
        };
        var executor = new Executor(Guid.NewGuid())
        {
            Name = "Executor",
            Email = "executor@gmail.ru",
            PhoneNumber = "456",
            UserType = UserType.Executor,
        };

        var order = new Order(Guid.NewGuid());

        var subject = new Subject(Guid.NewGuid())
        {
            Name = "Math"
        };

        customer.CurrentOrder = order;
        customer.CurrentOrderId = order.Id;

        executor.CurrentOrder = order;
        executor.CurrentOrderId = order.Id;
        order.AddWorkTemplate(new WorkTemplate(Guid.NewGuid())
        {
            Title = "title",
            Description = "description",
            WorkType = WorkType.CourseWork,
            Price = 1000,
            Subject = subject,
            SubjectId = subject.Id,
        });
        context.Orders.Add(order);
        Assert.That(context.SaveChanges(), Is.AtLeast(1));
    }

}
