using Newtonsoft.Json;
using ScienceAtrium.Domain.OrderAggregate;

namespace ScienceAtrium.Domain.UserAggregate.CustomerAggregate;

public class Customer : User
{
	public Customer(Guid id) : base(id, UserType.Customer)
	{
	}

	[JsonConstructor]
	public Customer(
		Guid id,
		string name,
		string email,
		string phoneNumber,
		List<Order> orders)
		: base(id, name, email, phoneNumber, UserType.Customer, orders)
	{
	}
}