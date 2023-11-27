using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ScienceAtrium.Domain.Constants;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Domain.RootAggregate;
using System.Diagnostics;

namespace ScienceAtrium.Domain.UserAggregate;

public class User : Entity
{
    public static readonly User Default = new User(Guid.Empty);
	private string _name;
    private string _email;
    private string _phoneNumber;
    private UserType _userType;
	private List<Order> _currentOrders;

	public User(Guid id, string name, string email, string phoneNumber, UserType userType) : base(id)
    {
        _name = name;
        _email = email;
        _phoneNumber = phoneNumber;
        _userType = userType;
        _currentOrders = new();
	}

    public User(Guid id) : base(id)
    {
        _currentOrders = new();
	}
    public string Name => _name;
    public string Email => _email;
    public string PhoneNumber => _phoneNumber;
    public UserType UserType => _userType;
    public Order? CurrentOrder { get; private set; }
    public Guid? CurrentOrderId { get; private set; }
    public IReadOnlyCollection<Order> Orders => _currentOrders;

    public virtual User UpdateCurrentOrder(Order? currentOrder, EntityState entityState = EntityState.Added)
    {
        if (entityState == EntityState.Added)
            AddOrder(currentOrder);
        else if (entityState == EntityState.Modified)
            UpdateOrder(x => x.Id == currentOrder.Id, currentOrder);
        else if (entityState == EntityState.Deleted)
            RemoveOrder(x => x.Id == currentOrder.Id);
        else
            AddOrder(currentOrder);

		CurrentOrder = currentOrder;
		CurrentOrderId = currentOrder?.Id;
		return this;
	}

    public User UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            Debug.Fail(DebugExceptions.NullOrWhiteSpace(nameof(name)));
            return this;
        }

        _name = name;
        return this;
    }

    public User UpdateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            Debug.Fail(DebugExceptions.NullOrWhiteSpace(nameof(email)));
            return this;
        }

        _email = email;
        return this;
    }

    public User UpdatePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            Debug.Fail(DebugExceptions.NullOrWhiteSpace(nameof(phoneNumber)));
            return this;
        }

        _phoneNumber = phoneNumber;
        return this;
    }

    public User UpdateUserType(UserType userType)
    {
        _userType = userType;
        return this;
    }

    public TUser MapTo<TUser>() where TUser : User
    {
        return new MapperConfiguration(mc => mc.CreateMap<User, TUser>())
            .CreateMapper()
            .Map<TUser>(this);
    }

    protected List<Order> AddOrder(Order order)
    {
        if (order?.IsEmpty() != false)
			return _currentOrders;

		var existOrder = _currentOrders.Find(x => x.Id == order.Id);
        if (existOrder?.IsEmpty() != false)
			return _currentOrders;

		_currentOrders.Add(order);
        return _currentOrders;
    }

    protected List<Order> RemoveOrder(Func<Order, bool> funcGetOrder)
    {
        var order = _currentOrders.FirstOrDefault(funcGetOrder);
        if (order?.IsEmpty() != false)
			return _currentOrders;

		_currentOrders.Remove(order);
        return _currentOrders;
    }

    protected List<Order> UpdateOrder(Func<Order, bool> funcGetOrder, Order newOrder)
    {
        var order = _currentOrders.FirstOrDefault(funcGetOrder);
        if (order?.IsEmpty() != false)
			return _currentOrders;

		_currentOrders.Remove(order);
        _currentOrders.Add(newOrder);
        return _currentOrders;
    }
}
