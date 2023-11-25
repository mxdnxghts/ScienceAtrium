using ScienceAtrium.Domain.OrderAggregate;

namespace ScienceAtrium.Domain.UserAggregate.ExecutorAggregate;
public class Executor : User
{
    private List<Order> _doneOrders;
    public Executor(Guid id) : base(id)
    {
        _doneOrders = new();
    }

    public IReadOnlyCollection<Order> DoneOrders => _doneOrders;

	public override User UpdateCurrentOrder(Order? currentOrder)
	{
		base.UpdateCurrentOrder(currentOrder);
        AddDoneOrder(currentOrder);
		return this;
	}

	public Executor AddDoneOrder(Order order)
    {
        _doneOrders = AddOrder(order);
        return this;
    }

    public Executor RemoveDoneOrder(Func<Order, bool> funcGetOrder)
    {
        _doneOrders = RemoveOrder(funcGetOrder);
        return this;
    }

    public Executor UpdateDoneOrder(Func<Order, bool> funcGetOrder, Order newOrder)
    {
        _doneOrders = UpdateOrder(funcGetOrder, newOrder);
        return this;
    }
}
