namespace ScienceAtrium.Domain.UserAggregate.CustomerAggregate;

public class Customer : User
{
    public Customer(Guid id) : base(id, UserType.Customer)
    {
    }
}
