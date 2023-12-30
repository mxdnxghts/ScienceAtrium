using AutoMapper;
using Infrastructure.IntegrationTests.Extensions;
using Microsoft.EntityFrameworkCore;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;
using ScienceAtrium.Domain.UserAggregate.ExecutorAggregate;
using ScienceAtrium.Domain.WorkTemplateAggregate;
using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Infrastructure.Extensions;
using ScienceAtrium.Infrastructure.UserAggregate;

namespace Infrastructure.IntegrationTests.Repositories.UserAggregate;
#pragma warning disable NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method
public class UserRepositoryTests
{
    private IUserRepository<Customer> _userRepository;
    private ApplicationContext _applicationContext;
    private List<string> _names;
    [SetUp]
    public void Setup()
    {
		_applicationContext = new ApplicationContext(
			new DbContextOptionsBuilder<ApplicationContext>()
			//.UseNpgsql("Server=localhost;Port=5432;Database=ScienceAtrium;User Id=postgres;Password=;Include Error Detail=true")
			.UseSqlServer("Server=localhost\\SQLEXPRESS;Data Source=maxim;Initial Catalog=Test;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False; Encrypt=True;TrustServerCertificate=True")
			.Options);

		var mapper = new MapperConfiguration(mc =>
        {
            mc.CreateMap<User, Customer>();
            mc.CreateMap<User, Executor>();
        }).CreateMapper();
		_userRepository = new UserRepository<Customer>(_applicationContext, null, mapper);

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
    public void A()
    {
        var subjects = new Subject[]
        {
            new(Guid.NewGuid())
            {
                Name = "Технологии разработки ПО"
            },
            new(Guid.NewGuid())
            {
                Name = "Операционные системы и среды"
            },
            new(Guid.NewGuid())
            {
                Name = "Математическое моделирование"
            },
        };

        var workTemplates = new WorkTemplate[]
        {
            new WorkTemplate(Guid.NewGuid())
            .UpdateTitle(subjects[0].Name)
            .UpdateSubject(subjects[0])
            .UpdateWorkType(WorkType.LaboratoryWork)
            .UpdatePrice(1200),
            new WorkTemplate(Guid.NewGuid())
            .UpdateTitle(subjects[1].Name)
            .UpdateSubject(subjects[1])
            .UpdateWorkType(WorkType.LaboratoryWork)
            .UpdatePrice(1200),
            new WorkTemplate(Guid.NewGuid())
            .UpdateTitle(subjects[2].Name)
            .UpdateSubject(subjects[2])
            .UpdateWorkType(WorkType.LaboratoryWork)
            .UpdatePrice(1200),
        };

        _applicationContext.Database.EnsureDeleted();
        _applicationContext.Database.EnsureCreated();

        _applicationContext.WorkTemplates.AddRange(workTemplates);
        var result = _applicationContext.TrySaveChanges(null);

		Assert.That(result, Is.AtLeast(1));
    }

    [Test]
    public void GetUserTest()
    {
        var user = _userRepository.Get(x =>
            x.Id != Guid.Empty);

        Assert.That(user,
            Is.Not.EqualTo(User.Default));
    }

    [Test]
    public void CreateUserTest()
    {
        var user = GetCustomerEntity();

        _userRepository.Create(user);
        Assert.That(_userRepository.Get(x => x.Id == user.Id),
            Is.Not.EqualTo(User.Default));
    }

    [Test]
    public void DeleteUserTest()
    {
        var user = _userRepository.All.ToList()[0];

        _userRepository.Delete(user);
        Assert.That(_userRepository.Get(x => x.Id == user.Id),
            Is.EqualTo(User.Default));
    }

    [Test]
    public void UpdateUserTest()
    {
        var user = _userRepository.All.ToList()[0];
        var oldUser = _userRepository.All.ToList()[0];
        user.UpdateName("New name");

        _userRepository.Update(user);

        Assert.That(_userRepository.Get(x => x.Id == user.Id).Name,
            Is.Not.EqualTo(oldUser.Name));
    }

    [Test]
    public void PrepareTests()
    {
		//TestExtension.PrepareTests<Customer, Entity>(_applicationContext,
		//    GetCustomerEntities(200, UserType.Customer), ensureDeleted: false, ensureCreated: false);
		//TestExtension.PrepareTests<Executor, Entity>(_applicationContext,
		//    GetExecutorEntities(200, UserType.Executor), ensureDeleted: false, ensureCreated: false);

        var subjects = new Subject[]
		{
			new(Guid.NewGuid())
			{
				Name = "Технологии разработки ПО"
			},
			new(Guid.NewGuid())
			{
				Name = "Операционные системы и среды"
			},
			new(Guid.NewGuid())
			{
				Name = "Математическое моделирование"
			},
		};

		var workTemplates = new WorkTemplate[]
		{
			new WorkTemplate(Guid.NewGuid())
			.UpdateTitle(subjects[0].Name)
			.UpdateSubject(subjects[0])
			.UpdateWorkType(WorkType.LaboratoryWork)
			.UpdatePrice(1200),
			new WorkTemplate(Guid.NewGuid())
			.UpdateTitle(subjects[1].Name)
			.UpdateSubject(subjects[1])
			.UpdateWorkType(WorkType.LaboratoryWork)
			.UpdatePrice(1200),
			new WorkTemplate(Guid.NewGuid())
			.UpdateTitle(subjects[2].Name)
			.UpdateSubject(subjects[2])
			.UpdateWorkType(WorkType.LaboratoryWork)
			.UpdatePrice(1200),
		};

		_applicationContext.Database.EnsureDeleted();
		_applicationContext.Database.EnsureCreated();

		_applicationContext.WorkTemplates.AddRange(workTemplates);
		var wtSubjects = _applicationContext.TrySaveChanges(null);

		var users = GetUserEntities(200);

        _applicationContext.Users.AddRange(users.Item1);
        _applicationContext.Users.AddRange(users.Item2);
        var savedUsers = _applicationContext.TrySaveChanges(null);


        Assert.That(wtSubjects, Is.AtLeast(1));
		Assert.That(savedUsers, Is.AtLeast(1));
	}

    private User GetUserEntity(UserType? userType)
	{
		return new User(Guid.NewGuid(), userType ?? (UserType)Random.Shared.Next(0, 1))
			.UpdateName(TestExtension.GetRandomName(_names))
			.UpdateEmail(TestExtension.GetRandomEmail(_names))
			.UpdatePhoneNumber(TestExtension.GetRandomPhoneNumber())
			.UpdateUserType(UserType.Customer);
	}

    private Customer GetCustomerEntity()
	{
		return new Customer(Guid.NewGuid())
			.UpdateName(TestExtension.GetRandomName(_names))
			.UpdateEmail(TestExtension.GetRandomEmail(_names))
			.UpdatePhoneNumber(TestExtension.GetRandomPhoneNumber())
			.UpdateUserType(UserType.Customer) as Customer;
	}

    private Executor GetExecutorEntity()
	{
		return new Executor(Guid.NewGuid())
			.UpdateName(TestExtension.GetRandomName(_names))
			.UpdateEmail(TestExtension.GetRandomEmail(_names))
			.UpdatePhoneNumber(TestExtension.GetRandomPhoneNumber())
			.UpdateUserType(UserType.Executor) as Executor;
	}

    private (Customer[], Executor[]) GetUserEntities(int usersCount)
    {
        var customers = new Customer[usersCount];
        var executors = new Executor[usersCount];
        for (int i = 0; i < usersCount; i++)
        {
            customers[i] = GetCustomerEntity();
            executors[i] = GetExecutorEntity();
        }
        return (customers, executors);
    }

    private User[] GetUserEntities(int usersCount, UserType? userType = null)
    {
        var users = new User[usersCount];
        for (int i = 0; i < usersCount; i++)
            users[i] = GetUserEntity(userType);
        return users;
    }
}
