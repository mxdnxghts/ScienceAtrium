using ScienceAtrium.Domain.OrderAggregate;

namespace ScienceAtrium.Domain.UserAggregate.CustomerAggregate;
public class CustomerJson : UserJson
{
	public CustomerJson(Guid id) : base(id)
	{
	}

	public List<Order> FormedOrders { get; set; }
}
