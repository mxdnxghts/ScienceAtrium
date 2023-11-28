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

	public Customer AddFormedOrder(Order order)
    {
		AddOrder(order);
		_formedOrders.AddRange(Orders);
		_formedOrders = _formedOrders.Distinct().ToList();
		return this;
    }

    public Customer RemoveFormedOrder(Func<Order, bool> funcGetOrder)
	{
		RemoveOrder(funcGetOrder);
		_formedOrders.AddRange(Orders);
		_formedOrders = _formedOrders.Distinct().ToList();
		return this;
    }

    public Customer UpdateFormedOrder(Func<Order, bool> funcGetOrder, Order newOrder)
	{
		UpdateOrder(funcGetOrder, newOrder);
		_formedOrders.AddRange(Orders);
		_formedOrders = _formedOrders.Distinct().ToList();
		return this;
    }
}
