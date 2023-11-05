using AutoMapper;
using Infrastructure.IntegrationTests.Extensions;
using Microsoft.EntityFrameworkCore;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;
using ScienceAtrium.Domain.WorkTemplateAggregate;
using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Domain.UserAggregate.ExecutorAggregate;
using ScienceAtrium.Infrastructure.Repositories.OrderAggregate;
using ScienceAtrium.Domain.RootAggregate;
using ScienceAtrium.Domain.RootAggregate.Interfaces;
using ScienceAtrium.Infrastructure.Repositories.UserAggregate;
using System.Linq.Expressions;

namespace Infrastructure.IntegrationTests.Repositories.OrderAggregate;

public class OrderRepositoryTest
{
    private IOrderRepository<Order> _orderRepository;
    private IBase<Customer> _customerBase;
    private IReader<Customer> _customerReader;
    private IBase<Executor> _executorBase;
    private IReader<Executor> _executorReader;
    private ApplicationContext _applicationContext;
    private IMapper _mapper;
    private List<string> _names;
    private Expression<Func<User, bool>> _expression;

    [SetUp]
    public void Setup()
    {
        _applicationContext = new ApplicationContext(
            new DbContextOptionsBuilder<ApplicationContext>()
            .UseNpgsql("Server=localhost;Port=5432;Database=ScienceAtrium;User Id=postgres;Password=;Include Error Detail=true").Options);

        _orderRepository = new OrderRepository(_applicationContext, null);

        _customerBase = new UserRepository<Customer>(_applicationContext, null);
        _customerReader = new UserRepository<Customer>(_applicationContext, null);
        _executorBase = new UserRepository<Executor>(_applicationContext, null);
        _executorReader = new UserRepository<Executor>(_applicationContext, null);
        _mapper = new MapperConfiguration(mc =>
        {
            mc.CreateMap<User, Customer>();
            mc.CreateMap<User, Executor>();
        }).CreateMapper();

        _expression = x => x.Id != Guid.Empty && x.CurrentOrderId == null;

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
        order.UpdateStatus(Status.Expired);

        _orderRepository.Update(order);

        Assert.That(_orderRepository.Get(x => x.Id == order.Id).Status,
            Is.Not.EqualTo(Order.Default.Status));
    }

    [Test]
    public async Task UpdateOrderAsyncTest()
    {
        var order = _orderRepository.All
            .FirstOrDefault(x => x.CustomerId != null && x.ExecutorId != null);
        order.UpdateStatus(Status.Expired);

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
                new Subject[] { subject },
            ensureDeleted: false);

        subject = _applicationContext.Subjects.FirstOrDefault(x => x.Id == subject.Id);
        TestExtension.PrepareTests<WorkTemplate, Entity>(_applicationContext,
            new WorkTemplate[] { workTemplate }, ensureDeleted: false);

        var orders = GetOrderEntities(100);

        _applicationContext.AttachRange(orders.Select(x => x.Customer));
        _applicationContext.AttachRange(orders.Select(x => x.Executor));

        TestExtension.PrepareTests<Order, Entity>(_applicationContext,
            orders, ensureDeleted: false);

        Assert.Pass();
    }

    private Order MapOrder()
    {
        var customer = _applicationContext
            .Set<Customer>().Include(x => x.CurrentOrder).AsNoTracking().AsEnumerable()
            .FirstOrDefault(x => x.UserType == UserType.Customer && x.CurrentOrder?.Id == null);

        var executor = _applicationContext
            .Set<Executor>().Include(x => x.CurrentOrder).AsNoTracking().AsEnumerable()
            .FirstOrDefault(x => x.UserType == UserType.Executor && x.CurrentOrder?.Id == null);
        return GetOrderEntity(
            _mapper.Map<Customer>(customer),
            _mapper.Map<Executor>(executor));
    }

    private Order GetOrderEntity(
        Customer? customer = null,
        Executor? executor = null,
        int position = default)
    {
        customer ??= _customerBase.All.Where(_expression).ToList()[position]
            .MapTo<Customer>();
        executor ??= _executorBase.All.Where(_expression).ToList()[position]
            .MapTo<Executor>();

        var order = new Order(Guid.NewGuid());

        // due to we create new customer and executor which don't exist in db yet
        // method IsValid returns false
        // supposed solution is to get real users from db
        order.UpdateCustomer(_customerReader, customer);
        order.UpdateExecutor(_executorReader, executor);

        customer.CurrentOrder = order;
        customer.CurrentOrderId = order.Id;

        executor.CurrentOrder = order;
        executor.CurrentOrderId = order.Id;

        order.AddWorkTemplate(_applicationContext.WorkTemplates
            .SingleOrDefault(x => x.WorkType == WorkType.CourseWork));

        return order;
    }

    private Order[] GetOrderEntities(int ordersCount)
    {
        var orders = new Order[ordersCount];
        for (int i = 0; i < ordersCount; i++)
            orders[i] = GetOrderEntity(position: i);
        return orders;
    }

    private (Customer, Executor) GetNewCustomerExecutor()
    {
        return new(new Customer(Guid.NewGuid())
        {
            Name = TestExtension.GetRandomName(_names),
            Email = TestExtension.GetRandomEmail(_names),
            PhoneNumber = TestExtension.GetRandomPhoneNumber(),
            UserType = UserType.Customer,
        },
        new Executor(Guid.NewGuid())
        {
            Name = TestExtension.GetRandomName(_names),
            Email = TestExtension.GetRandomEmail(_names),
            PhoneNumber = TestExtension.GetRandomPhoneNumber(),
            UserType = UserType.Executor,
        });
    }
}
