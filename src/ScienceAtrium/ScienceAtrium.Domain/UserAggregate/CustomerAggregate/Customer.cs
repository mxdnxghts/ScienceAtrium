
using ScienceAtrium.Domain.OrderAggregate;

namespace ScienceAtrium.Domain.UserAggregate.CustomerAggregate;
public class Customer : User
{
    public Customer(Guid id) : base(id)
    {
    }
    public List<Order> FormerOrders { get; private set; } = new();
}
