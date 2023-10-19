using AutoMapper;
using Infrastructure.IntegrationTests.Extensions;
using Microsoft.EntityFrameworkCore;
using ScienceAtrium.Domain.Entities;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;
using ScienceAtrium.Domain.WorkTemplateAggregate;
using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Domain.UserAggregate.ExecutorAggregate;
using ScienceAtrium.Infrastructure.Repositories.OrderAggregate;

namespace Infrastructure.IntegrationTests.Repositories.OrderAggregate;

public class OrderRepositoryTest
{
    private IOrderRepository<Order> _orderRepository;
    private ApplicationContext _applicationContext;
    private IMapper _mapper;
    private List<string> _names;
    [SetUp]
    public void Setup()
    {
        _applicationContext = new ApplicationContext(
            new DbContextOptionsBuilder<ApplicationContext>()
            .UseNpgsql("Server=localhost;Port=5432;Database=ScienceAtrium;User Id=postgres;Password=;Include Error Detail=true").Options);

        _orderRepository = new OrderRepository(_applicationContext, null);
        _mapper = new MapperConfiguration(mc =>
        {
            mc.CreateMap<User, Customer>();
            mc.CreateMap<User, Executor>();
        }).CreateMapper();

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
    public async Task GetOrderAsyncTest()
    {
        var order = await _orderRepository.GetAsync(x =>
            x.Id != Guid.Empty);

        Assert.That(order,
            Is.Not.EqualTo(Order.Default));
    }

    [Test]
    public void CreateOrderTest()
    {
        var order = MapOrder();
        _orderRepository.Create(order);
        Assert.That(_orderRepository.Get(x => x.Id == order.Id),
            Is.Not.EqualTo(Order.Default));
    }

    [Test]
    public async Task CreateOrderAsyncTest()
    {
        var order = MapOrder();
        await _orderRepository.CreateAsync(order);
        Assert.That(_orderRepository.Get(x => x.Id == order.Id),
            Is.Not.EqualTo(Order.Default));
    }

    [Test]
    public void DeleteOrderTest()
    {
        var order = _orderRepository.All
            .FirstOrDefault(x => x.CustomerId != null && x.ExecutorId != null);

        _orderRepository.Delete(order);
        Assert.That(_orderRepository.Get(x => x.Id == order.Id),
            Is.EqualTo(Order.Default));
    }

    [Test]
    public async Task DeleteOrderAsyncTest()
    {
        var order = _orderRepository.All
            .FirstOrDefault(x => x.CustomerId != null && x.ExecutorId != null);

        await _orderRepository.DeleteAsync(order);
        Assert.That(_orderRepository.Get(x => x.Id == order.Id),
            Is.EqualTo(Order.Default));
    }

    [Test]
    public void UpdateOrderTest()
    {
        var order = _orderRepository.All
            .FirstOrDefault(x => x.CustomerId != null && x.ExecutorId != null);
        order.Status = Status.Expired;

        _orderRepository.Update(order);

        Assert.That(_orderRepository.Get(x => x.Id == order.Id).Status,
            Is.Not.EqualTo(Order.Default.Status));
    }

    [Test]
    public async Task UpdateOrderAsyncTest()
    {
        var order = _orderRepository.All
            .FirstOrDefault(x => x.CustomerId != null && x.ExecutorId != null);
        order.Status = Status.Expired;

        await _orderRepository.UpdateAsync(order);

        Assert.That(_orderRepository.Get(x => x.Id == order.Id).Status,
            Is.Not.EqualTo(Order.Default.Status));
    }

    [Test]
    public void PrepareTests()
    {
        var subject = new Subject(new Guid("{628F92CF-93C2-4F74-98BB-18A96765D40F}"))
        {
            Name = "Math"
        };
        var workTemplate = new WorkTemplate(
            new Guid("{40405132-7516-4731-86E3-B6D4A6D956E7}"))
        {
            Title = $"{TestExtension.GetRandomEmail(_names)}-title",
            Description = $"{TestExtension.GetRandomEmail(_names)}-description",
            WorkType = WorkType.CourseWork,
            Price = Random.Shared.Next(1000, 10_000),
            Subject = subject,
            SubjectId = subject.Id,
        };
        TestExtension.PrepareTests<Subject, Entity>(
            _applicationContext,
                new Subject[] { subject }
            );

        subject = _applicationContext.Subjects.FirstOrDefault(x => x.Id == subject.Id);
        TestExtension.PrepareTests<WorkTemplate, Entity>(_applicationContext,
            new WorkTemplate[] { workTemplate }, ensureDeleted: false);

        TestExtension.PrepareTests<Order, Entity>(_applicationContext, 
            GetOrderEntities(100), ensureDeleted: false);

        Assert.Pass();
    }

    private Order MapOrder()
    {
        var customer = _applicationContext
            .Users.AsNoTracking().AsEnumerable()
            .FirstOrDefault(x => x.UserType == UserType.Customer && x.CurrentOrderId == null);

        var executor = _applicationContext
            .Users.AsNoTracking().AsEnumerable()
            .FirstOrDefault(x => x.UserType == UserType.Executor && x.CurrentOrderId == null);
        return GetOrderEntity(
            _mapper.Map<Customer>(customer),
            _mapper.Map<Executor>(executor));
    }

    private Order GetOrderEntity(
        Customer? customer = null,
        Executor? executor = null)
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

        customer.CurrentOrder = order;
        customer.CurrentOrderId = order.Id;

        executor.CurrentOrder = order;
        executor.CurrentOrderId = order.Id;

        order.CustomerId = order.Customer.Id;
        order.ExecutorId = order.Executor.Id;

        order.WorkTemplates.Add(_applicationContext.WorkTemplates
            .SingleOrDefault(x => x.WorkType == WorkType.CourseWork));

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
