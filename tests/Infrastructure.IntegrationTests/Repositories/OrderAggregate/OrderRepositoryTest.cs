using Infrastructure.IntegrationTests.Extensions;
using Microsoft.EntityFrameworkCore;
using ScienceAtrium.Application.Common.Interfaces;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;
using ScienceAtrium.Domain.UserAggregate.Executor;
using ScienceAtrium.Domain.WorkTemplateAggregate;
using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Infrastructure.Extensions;
using ScienceAtrium.Infrastructure.Repositories.OrderAggregation;
using System.Text;

namespace Infrastructure.IntegrationTests.Repositories.OrderAggregate;

public class OrderRepositoryTest
{
    private IOrderRepository<Order> _orderRepository;
    private ApplicationContext _applicationContext;
    private List<string> _names;
    [SetUp]
    public void Setup()
    {
        _applicationContext = new ApplicationContext(
            new DbContextOptionsBuilder<ApplicationContext>()
            .UseNpgsql("Server=localhost;Port=5432;Database=ScienceAtrium;User Id=postgres;Password=;Include Error Detail=true").Options);

        _orderRepository = new OrderRepository(_applicationContext, null);

        _names = new List<string>
        {
            "Nick",
            "Tom",
            "John",
            "Alex",
            "Maxim",
        };

        _applicationContext.Database.EnsureCreated();
    }

    [Test]
    public void GetOrderTest()
    {
        var order = _orderRepository.Get(x =>
            x.Id != Guid.Empty);

        Assert.That(order,
            Is.Not.EqualTo(Order.Default));
    }

    [Test]
    public void CreateOrderTest()
    {
        var order = GetOrderEntity(
            (Customer)_applicationContext.Users.FirstOrDefault(x => x.UserType == UserType.Customer),
            (Executor)_applicationContext.Users.FirstOrDefault(x => x.UserType == UserType.Executor));

        _orderRepository.Create(order);
        Assert.That(_orderRepository.Get(x => x.Id == order.Id),
            Is.Not.EqualTo(Order.Default));
    }

    [Test]
    public void DeleteOrderTest()
    {
        var order = _orderRepository.All.ToList()[0];

        _orderRepository.Delete(order);
        Assert.That(_orderRepository.Get(x => x.Id == order.Id),
            Is.EqualTo(Order.Default));
    }

    [Test]
    public void UpdateOrderTest()
    {
        var order = _orderRepository.All.ToList()[0];
        order.Status = Status.Expired;

        _orderRepository.Update(order);

        Assert.That(_orderRepository.Get(x => x.Id == order.Id).Status,
            Is.Not.EqualTo(Order.Default.Status));
    }

    [Test]
    public void PrepareTests()
    {
        TestExtension.PrepareTests(_applicationContext, GetOrderEntities(100));
    }



    private Order GetOrderEntity(
        Customer? customer = null,
        Executor? executor = null,
        Subject? subject = null)
    {
        customer ??= new Customer(Guid.NewGuid())
        {
            Name = TestExtension.GetRandomName(_names),
            Email = TestExtension.GetRandomEmail(_names),
            PhoneNumber = TestExtension.GetRandomPhoneNumber(),
            UserType = UserType.Customer,
        };
        executor ??= new Executor(Guid.NewGuid())
        {
            Name = TestExtension.GetRandomName(_names),
            Email = TestExtension.GetRandomEmail(_names),
            PhoneNumber = TestExtension.GetRandomPhoneNumber(),
            UserType = UserType.Executor,
        };

        var order = new Order(Guid.NewGuid())
        {
            Customer = customer,
            Executor = executor,
        };

        subject ??= new Subject(Guid.NewGuid())
        {
            Name = "Math"
        };

        customer.CurrentOrder = order;
        customer.CurrentOrderId = order.Id;

        executor.CurrentOrder = order;
        executor.CurrentOrderId = order.Id;

        order.CustomerId = order.Customer.Id;
        order.ExecutorId = order.Executor.Id;
        order.WorkTemplates.Add(new WorkTemplate(Guid.NewGuid())
        {
            Title = $"{TestExtension.GetRandomEmail(_names)}title",
            Description = $"{TestExtension.GetRandomEmail(_names)}description",
            WorkType = WorkType.CourseWork,
            Price = Random.Shared.Next(1000, 10_000),
            Subject = subject,
            SubjectId = subject.Id,
        });
        return order;
    }

    private Order[] GetOrderEntities(int ordersCount)
    {
        var orders = new Order[ordersCount];
        for (int i = 0; i < ordersCount; i++)
            orders[i] = GetOrderEntity();
        return orders;
    }
}
