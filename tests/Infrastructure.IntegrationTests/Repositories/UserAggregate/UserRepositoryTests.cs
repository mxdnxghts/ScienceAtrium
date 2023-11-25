using Infrastructure.IntegrationTests.Extensions;
using Microsoft.EntityFrameworkCore;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;
using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Domain.UserAggregate.ExecutorAggregate;
using ScienceAtrium.Infrastructure.Repositories.UserAggregate;
using ScienceAtrium.Domain.RootAggregate;
using ScienceAtrium.Domain.WorkTemplateAggregate;
using ScienceAtrium.Infrastructure.Extensions;

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
            .UseNpgsql("Server=localhost;Port=5432;Database=ScienceAtrium;User Id=postgres;Password=;Include Error Detail=true").Options);
        
        _userRepository = new UserRepository<Customer>(_applicationContext, null);

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

        _applicationContext.WorkTemplates.AddRange(workTemplates);
        _applicationContext.TrySaveChanges(null);
        Assert.Pass();
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
            Is.EqualTo(User.Default.MapTo<Customer>()));
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

        _applicationContext.Database.EnsureDeleted();
        _applicationContext.Database.EnsureCreated();

        _applicationContext.Users.AddRange(GetCustomerEntities(200));
        _applicationContext.Users.AddRange(GetExecutorEntities(200));
        _applicationContext.TrySaveChanges(null);


        Assert.Pass();
    }

    private Customer GetCustomerEntity(UserType? userType = null)
    {
        return new Customer(Guid.NewGuid())
            .UpdateName(TestExtension.GetRandomName(_names))
            .UpdateEmail(TestExtension.GetRandomEmail(_names))
            .UpdatePhoneNumber(TestExtension.GetRandomPhoneNumber())
            .UpdateUserType(UserType.Customer) as Customer;
    }

    private Executor GetExecutorEntity(UserType? userType = null)
    {
        return new Executor(Guid.NewGuid())
            .UpdateName(TestExtension.GetRandomName(_names))
            .UpdateEmail(TestExtension.GetRandomEmail(_names))
            .UpdatePhoneNumber(TestExtension.GetRandomPhoneNumber())
            .UpdateUserType(UserType.Executor) as Executor;
    }

    private Customer[] GetCustomerEntities(int usersCount, UserType? userType = null)
    {
        var users = new Customer[usersCount];
        for (int i = 0; i < usersCount; i++)
            users[i] = GetCustomerEntity(userType);
        return users;
    }

    private Executor[] GetExecutorEntities(int usersCount, UserType? userType = null)
    {
        var users = new Executor[usersCount];
        for (int i = 0; i < usersCount; i++)
            users[i] = GetExecutorEntity(userType);
        return users;
    }
}
