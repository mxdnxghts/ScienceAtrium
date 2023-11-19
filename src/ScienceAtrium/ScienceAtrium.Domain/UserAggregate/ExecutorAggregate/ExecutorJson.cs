using ScienceAtrium.Domain.OrderAggregate;

namespace ScienceAtrium.Domain.UserAggregate.ExecutorAggregate;
public class ExecutorJson : UserJson
{
	public ExecutorJson(Guid id) : base(id)
	{
	}

	public List<Order> DoneOrders { get; set; }
}
