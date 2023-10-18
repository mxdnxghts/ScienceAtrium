using Infrastructure.IntegrationTests.Extensions;
using Microsoft.EntityFrameworkCore;
using ScienceAtrium.Application.Common.Interfaces;
using ScienceAtrium.Domain.Entities;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Infrastructure.Repositories.UserAggregation;

namespace Infrastructure.IntegrationTests.Repositories.UserAggregate;

public class UserRepositoryTests
{
    private IUserRepository<User> _userRepository;
    private ApplicationContext _applicationContext;
    private List<string> _names;
    [SetUp]
    public void Setup()
    {
        _applicationContext = new ApplicationContext(
            new DbContextOptionsBuilder<ApplicationContext>()
            .UseNpgsql("Server=localhost;Port=5432;Database=ScienceAtrium;User Id=postgres;Password=;Include Error Detail=true").Options);

        _userRepository = new UserRepository<User>(_applicationContext, null);

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
        var user = GetUserEntity();

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
        user.Name = "New name";

        _userRepository.Update(user);

        Assert.That(_userRepository.Get(x => x.Id == user.Id).Name,
            Is.Not.EqualTo(oldUser.Name));
    }

    [Test]
    public void PrepareTests()
    {
        TestExtension.PrepareTests<User, Entity>(_applicationContext,
            GetUserEntities(100, UserType.Customer), ensureDeleted: false);
        TestExtension.PrepareTests<User, Entity>(_applicationContext,
            GetUserEntities(100, UserType.Executor), ensureDeleted: false);

        Assert.Pass();
    }

    private User GetUserEntity(UserType? userType = null)
    {
        return new User(Guid.NewGuid())
        {
            Name = TestExtension.GetRandomName(_names),
            Email = TestExtension.GetRandomEmail(_names),
            PhoneNumber = TestExtension.GetRandomPhoneNumber(),
            UserType = userType ?? (UserType)Random.Shared.Next(0, 1)
        };
    }

    private User[] GetUserEntities(int usersCount, UserType? userType = null)
    {
        var users = new User[usersCount];
        for (int i = 0; i < usersCount; i++)
            users[i] = GetUserEntity(userType);
        return users;
    }
}
