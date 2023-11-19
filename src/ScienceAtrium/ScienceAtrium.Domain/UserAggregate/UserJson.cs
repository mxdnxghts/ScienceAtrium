using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Domain.RootAggregate;

namespace ScienceAtrium.Domain.UserAggregate;
public class UserJson : Entity
{
	public UserJson(Guid id) : base(id)
	{
	}

	public string Name { get; set; }
	public string Email { get; set; }
	public string PhoneNumber { get; set; }
	public UserType UserType { get; set; }
	public Order? CurrentOrder { get; set; }
	public Guid? CurrentOrderId { get; set; }
}
