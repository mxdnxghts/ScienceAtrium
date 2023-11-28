using Microsoft.EntityFrameworkCore;
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

	public Executor AddDoneOrder(Order order)
	{
		AddOrder(order);
		_doneOrders.AddRange(Orders);
		_doneOrders = _doneOrders.Distinct().ToList();
		return this;
	}

    public Executor RemoveDoneOrder(Func<Order, bool> funcGetOrder)
	{
		RemoveOrder(funcGetOrder);
		_doneOrders.AddRange(Orders);
		_doneOrders = _doneOrders.Distinct().ToList();
		return this;
	}

    public Executor UpdateDoneOrder(Func<Order, bool> funcGetOrder, Order newOrder)
	{
		UpdateOrder(funcGetOrder, newOrder);
		_doneOrders.AddRange(Orders);
		_doneOrders = _doneOrders.Distinct().ToList();
		return this;
	}
}
