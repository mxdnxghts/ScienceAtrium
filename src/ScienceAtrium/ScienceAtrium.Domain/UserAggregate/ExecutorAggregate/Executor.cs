
using ScienceAtrium.Domain.OrderAggregate;

namespace ScienceAtrium.Domain.UserAggregate.Executor;
public class Executor : User
{
    public Executor(Guid id) : base(id)
    {
    }
    public List<Order> DoneOrders { get; private set; } = new();
}
