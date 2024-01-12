using ScienceAtrium.Domain.Constants;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Domain.RootAggregate;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace ScienceAtrium.Domain.UserAggregate;

public class User : Entity
{
    public static readonly User Default = new User(Guid.Empty);
	private string _name;
    private string _email;
    private string _phoneNumber;
    private UserType _userType;
	private readonly List<Order> _currentOrders;
    public User(Guid id) : base(id)
    {
        _currentOrders = new();
    }

    public User(Guid id, UserType userType)
        : base(id)
    {
        _currentOrders = new();
        _userType = userType;
    }

    public User(Guid id, string name, string email, string phoneNumber, UserType userType)
        : base(id)
    {
        _name = name;
        _email = email;
        _phoneNumber = phoneNumber;
        _userType = userType;
        _currentOrders = new();
	}

    protected User(Guid id,
        string name,
        string email,
        string phoneNumber,
        UserType userType,
        List<Order> orders)
        : base(id)
    {
        _name = name;
        _email = email;
        _phoneNumber = phoneNumber;
        _userType = userType;
        _currentOrders = orders;
    }

    public string Name => _name;
    public string Email => _email;
    public string PhoneNumber => _phoneNumber;
    public UserType UserType => _userType;
    public IReadOnlyCollection<Order> Orders => _currentOrders;

    public User UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            Debug.Print(DebugExceptions.NullOrWhiteSpace(nameof(name)));
            return this;
        }

        _name = name;
        return this;
    }

    public User UpdateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            Debug.Print(DebugExceptions.NullOrWhiteSpace(nameof(email)));
            return this;
        }

        _email = email;
        return this;
    }

    public User UpdatePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            Debug.Print(DebugExceptions.NullOrWhiteSpace(nameof(phoneNumber)));
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

    public User AddOrder(Order order)
    {
        if (order?.IsEmpty() != false)
			return this;

		var existOrder = _currentOrders.Find(x => x.Id == order.Id);
        if (existOrder?.IsEmpty() == false)
			return this;

		_currentOrders.Add(order);
        return this;
    }

    public User RemoveOrder(Func<Order, bool> funcGetOrder)
    {
        var order = _currentOrders.FirstOrDefault(funcGetOrder);
        if (order?.IsEmpty() != false)
			return this;

		_currentOrders.Remove(order);
        return this;
    }

    public User UpdateOrder(Func<Order, bool> funcGetOrder, Order newOrder)
    {
        var order = _currentOrders.FirstOrDefault(funcGetOrder);
        if (order?.IsEmpty() != false)
			return this;

		_currentOrders.Remove(order);
        _currentOrders.Add(newOrder);
        return this;
    }
}
