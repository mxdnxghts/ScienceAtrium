using ScienceAtrium.Domain.Entities;
namespace ScienceAtrium.Domain.UserAggregate;

public class User : Entity
{
    public User(Guid id) : base(id)
    {
    }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public UserType UserType { get; set; }
}
public enum UserType
{
    Customer,
    Executor
}
