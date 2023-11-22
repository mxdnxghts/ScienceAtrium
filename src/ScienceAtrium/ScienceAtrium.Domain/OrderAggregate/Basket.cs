using ScienceAtrium.Domain.RootAggregate;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;

namespace ScienceAtrium.Domain.OrderAggregate;
public class Basket : Entity
{
    public Basket(Guid id) : base(id)
    {
    }

    public Customer? Customer { get; private set; }
    public Guid? Customerid { get; private set; }

    public Order? Order { get; private set; }
    public Guid? OrderId { get; private set; }

}
