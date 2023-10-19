using Infrastructure.IntegrationTests.Extensions;
using Microsoft.EntityFrameworkCore;
using ScienceAtrium.Application.Common.Interfaces;
using ScienceAtrium.Domain.Entities;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;
using ScienceAtrium.Domain.UserAggregate.Executor;
using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Infrastructure.Repositories.UserAggregation;

namespace Infrastructure.IntegrationTests.Repositories.UserAggregate;

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

        var s = _userRepository.All;

        _applicationContext.Database.EnsureCreated();
    }

    [Test]
    public void GetUserTest()
    {
        var order = _userRepository.Get(x =>
            x.Id != Guid.Empty);

        Assert.That(order,
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
            Is.EqualTo(User.MapTo<Customer>(User.Default)));
    }

    [Test]
    public void UpdateUserTest()
    {
        var user = _userRepository.All.ToList()[0];
        var oldUser = _userRepository.All.ToList()[0];
        user.Name = "New name";

        _userRepository.Update(user);

        Assert.That(_userRepository.Get(x => x.Id == user.Id).Name,
            Is.Not.EqualTo(oldUser.Name));
    }

    [Test]
    public void PrepareTests()
    {
        TestExtension.PrepareTests<Customer, Entity>(_applicationContext,
            GetUserEntities<Customer>(100, UserType.Customer), ensureDeleted: false);
        TestExtension.PrepareTests<Executor, Entity>(_applicationContext,
            GetUserEntities<Executor>(100, UserType.Executor), ensureDeleted: false);

        Assert.Pass();
    }

    private TUser GetUserEntity<TUser>(UserType? userType = null)
        where TUser : User
    {
        return User.MapTo<TUser>(new User(Guid.NewGuid())
        {
            Name = TestExtension.GetRandomName(_names),
            Email = TestExtension.GetRandomEmail(_names),
            PhoneNumber = TestExtension.GetRandomPhoneNumber(),
            UserType = userType ?? (UserType)Random.Shared.Next(0, 1)
        });
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
