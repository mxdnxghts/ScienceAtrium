using Infrastructure.IntegrationTests.Extensions;
using Microsoft.EntityFrameworkCore;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;
using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Domain.UserAggregate.ExecutorAggregate;
using ScienceAtrium.Infrastructure.Repositories.UserAggregate;
using ScienceAtrium.Domain.RootAggregate;

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
        var user = GetUserEntity<Customer>();

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
        TestExtension.PrepareTests<Customer, Entity>(_applicationContext,
            GetUserEntities<Customer>(200, UserType.Customer), ensureDeleted: true);
        TestExtension.PrepareTests<Executor, Entity>(_applicationContext,
            GetUserEntities<Executor>(200, UserType.Executor), ensureDeleted: false);

        Assert.Pass();
    }

    private TUser GetUserEntity<TUser>(UserType? userType = null)
        where TUser : User
    {
        return new User(
            id: Guid.NewGuid(),
            name: TestExtension.GetRandomName(_names),
            email: TestExtension.GetRandomEmail(_names),
            phoneNumber: TestExtension.GetRandomPhoneNumber(),
            userType: userType ?? (UserType)Random.Shared.Next(0, 1)).MapTo<TUser>();
    }

    private TUser[] GetUserEntities<TUser>(int usersCount, UserType? userType = null)
        where TUser : User
    {
        var users = new TUser[usersCount];
        for (int i = 0; i < usersCount; i++)
            users[i] = GetUserEntity<TUser>(userType);
        return users;
    }
}
