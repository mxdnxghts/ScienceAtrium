using Microsoft.EntityFrameworkCore;
using ScienceAtrium.Domain.OrderAggregate;

namespace ScienceAtrium.Domain.UserAggregate.CustomerAggregate;

public class Customer : User
{
	private List<Order> _formedOrders;
    
    public Customer(Guid id) : base(id)
    {
        _formedOrders = new();
    }

    public IReadOnlyCollection<Order> FormedOrders => _formedOrders;

	public override User UpdateCurrentOrder(Order? currentOrder, EntityState entityState = EntityState.Added)
	{
        base.UpdateCurrentOrder(currentOrder, entityState);
		_formedOrders.AddRange(Orders);
		_formedOrders = _formedOrders.Distinct().ToList();
        return this;
	}

	public Customer AddFormedOrder(Order order)
    {
        _formedOrders = AddOrder(order);
        return this;
    }

    public Customer RemoveFormedOrder(Func<Order, bool> funcGetOrder)
    {
        _formedOrders = RemoveOrder(funcGetOrder);
        return this;
    }

    public Customer UpdateFormedOrder(Func<Order, bool> funcGetOrder, Order newOrder)
    {
        _formedOrders = UpdateOrder(funcGetOrder, newOrder);
        return this;
    }
}
