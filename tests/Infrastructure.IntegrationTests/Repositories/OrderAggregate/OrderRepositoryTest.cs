using AutoMapper;
using Infrastructure.IntegrationTests.Extensions;
using Microsoft.EntityFrameworkCore;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Domain.RootAggregate;
using ScienceAtrium.Domain.RootAggregate.Interfaces;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;
using ScienceAtrium.Domain.UserAggregate.ExecutorAggregate;
using ScienceAtrium.Domain.WorkTemplateAggregate;
using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Infrastructure.OrderAggregate;
using ScienceAtrium.Infrastructure.UserAggregate;
using System.Linq.Expressions;

namespace Infrastructure.IntegrationTests.Repositories.OrderAggregate;
#pragma warning disable NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method
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
    private readonly object _lock = new();

    [SetUp]
    public void Setup()
    {
        _applicationContext = new ApplicationContext(GetDbContextOptions());

        _orderRepository = new OrderRepository(_applicationContext, null);
        _mapper = new MapperConfiguration(mc =>
        {
            //AddUserMapper(mc);
            mc.CreateMap<User, Customer>().ConstructUsing(user =>
            new Customer(user.Id)
            .UpdateName(user.Name)
            .UpdateEmail(user.Email)
            .UpdatePhoneNumber(user.PhoneNumber)
            .UpdateUserType(user.UserType) as Customer);
            mc.CreateMap<User, Executor>().ConstructUsing(user =>
            new Executor(user.Id)
            .UpdateName(user.Name)
            .UpdateEmail(user.Email)
            .UpdatePhoneNumber(user.PhoneNumber)
            .UpdateUserType(user.UserType) as Executor);
        }).CreateMapper();

        _customerBase = new UserRepository<Customer>(_applicationContext, null, _mapper);
        _customerReader = new UserRepository<Customer>(_applicationContext, null, _mapper);
        _executorBase = new UserRepository<Executor>(_applicationContext, null, _mapper);
        _executorReader = new UserRepository<Executor>(_applicationContext, null, _mapper);

        _expression = x => x.Id != Guid.Empty;

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
    public void CreateOrderWithSameCustomerTest()
    {
		_applicationContext = new ApplicationContext(GetDbContextOptions());
		var customer = _customerReader
            .Get(x => x.Id != Guid.Empty);

        var order = MapOrder(customer);
		_orderRepository.Create(order);

		_applicationContext = new ApplicationContext(GetDbContextOptions());

		var newOrder = MapOrder(customer);
        lock (_lock)
        {
			_orderRepository.Create(newOrder);
		}
		Assert.That(_orderRepository.Get(x => x.Id == order.Id),
			Is.Not.EqualTo(Order.Default));
	}

    [Test]
    public void CreateOrderWithSampleWorkTemplatesTest()
    {
        _applicationContext = new ApplicationContext(GetDbContextOptions());
        _orderRepository = new OrderRepository(_applicationContext, null);

        var order = MapOrder();
		_orderRepository.Create(order);

		var workTemplate = _applicationContext.WorkTemplates
			.Include(x => x.Subject)
			.AsNoTracking()
			.FirstOrDefault(x => x.Title.Trim().ToLower() == "Математическое моделирование".Trim().ToLower());
        var count = order.WorkTemplates.ToList().Count;
		order.AddWorkTemplate(workTemplate);

        var s = _orderRepository.Get(x => x.Id == order.Id);

		_applicationContext = new ApplicationContext(GetDbContextOptions());
        _orderRepository = new OrderRepository(_applicationContext, null);

		_orderRepository.Update(order);
		Assert.That(_orderRepository.Get(x => x.Id == order.Id).WorkTemplates.Count,
			Is.AtLeast(count + 1));
	}

    [Test]
    public void RemoveWorkTemplateTest()
    {
        var order = _orderRepository.Get(x => x.Id != Guid.Empty);
        var count = order.WorkTemplates.ToList().Count;
        order.RemoveWorkTemplate(x => x.Id != Guid.Empty);
        _orderRepository.Update(order);

        Assert.That(_orderRepository.Get(x => x.Id == order.Id).WorkTemplates.Count,
            Is.AtMost(count - 1));
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
        order.UpdateStatus(OrderStatus.Expired);

        _orderRepository.Update(order);

        Assert.That(_orderRepository.Get(x => x.Id == order.Id).Status,
            Is.Not.EqualTo(Order.Default.Status));
    }

    [Test]
    public async Task UpdateOrderAsyncTest()
    {
        var order = _orderRepository.All
            .FirstOrDefault(x => x.CustomerId != null && x.ExecutorId != null);
        order.UpdateStatus(OrderStatus.Expired);

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
        //var workTemplate = new WorkTemplate(
        //    id: new Guid("{40405132-7516-4731-86E3-B6D4A6D956E7}"),
        //    title: $"{TestExtension.GetRandomEmail(_names)}-title",
        //    description: $"{TestExtension.GetRandomEmail(_names)}-description",
        //    workType: WorkType.CourseWork,
        //    price: Random.Shared.Next(1000, 10_000)
        var workTemplate = new WorkTemplate(Guid.NewGuid()).UpdateSubject(subject);

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

    private Order MapOrder(Customer? customer = null)
    {
        customer ??= _customerReader.Get(x => x.UserType == UserType.Customer);

        var executor = _executorReader.Get(x => x.UserType == UserType.Executor);

		return GetOrderEntity(customer, executor);
    }

    private Order GetOrderEntity(
        Customer? customer = null,
        Executor? executor = null,
        int position = default)
    {
        customer ??= _customerBase.All
            .Where(x => x.UserType == UserType.Customer)
            .ToList()[position];

		executor ??= _executorBase.All
            .Where(x => x.UserType == UserType.Executor)
            .ToList()[position];

		var order = new Order(Guid.NewGuid())
            .UpdateCustomer(_customerReader, customer)
            .UpdateExecutor(_executorReader, executor)
            .AddWorkTemplate(_applicationContext.WorkTemplates.Include(x => x.Subject).AsNoTracking()
                .FirstOrDefault(x => x.WorkType == WorkType.LaboratoryWork));

		customer.AddOrder(order);
        executor.AddOrder(order);

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
        return new(new User(
            id: Guid.NewGuid(),
            name: TestExtension.GetRandomName(_names),
            email: TestExtension.GetRandomEmail(_names),
            phoneNumber: TestExtension.GetRandomPhoneNumber(),
            userType: UserType.Customer) as Customer,
        new User(
            id: Guid.NewGuid(),
            name: TestExtension.GetRandomName(_names),
            email: TestExtension.GetRandomEmail(_names),
            phoneNumber: TestExtension.GetRandomPhoneNumber(),
            userType: UserType.Executor) as Executor);
    }

	private IMapperConfigurationExpression AddUserMapper(IMapperConfigurationExpression mapperConfiguration)
	{
#pragma warning disable CS8603 // Possible null reference return.
		mapperConfiguration.CreateMap<User, Customer>().ConstructUsing(user =>
			new Customer(user.Id)
			.UpdateName(user.Name)
			.UpdateEmail(user.Email)
			.UpdatePhoneNumber(user.PhoneNumber)
			.UpdateUserType(user.UserType) as Customer);
		mapperConfiguration.CreateMap<User, Executor>().ConstructUsing(user =>
			new Executor(user.Id)
			.UpdateName(user.Name)
			.UpdateEmail(user.Email)
			.UpdatePhoneNumber(user.PhoneNumber)
			.UpdateUserType(user.UserType) as Executor);
#pragma warning restore CS8603 // Possible null reference return.
		return mapperConfiguration;
	}

    private DbContextOptions<ApplicationContext> GetDbContextOptions()
    {
        return new DbContextOptionsBuilder<ApplicationContext>()
            //.UseNpgsql("Server=localhost;Port=5432;Database=ScienceAtrium;User Id=postgres;Password=;Include Error Detail=true")
            .UseSqlServer("Server=localhost\\SQLEXPRESS;Data Source=maxim;Initial Catalog=Test;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False; Encrypt=True;TrustServerCertificate=True")
            .Options;
    }
}
