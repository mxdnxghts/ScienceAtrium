using AutoMapper;
using ScienceAtrium.Domain.Constants;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Domain.RootAggregate;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace ScienceAtrium.Domain.UserAggregate;

public class User : Entity
{
    [NotMapped]
    public static readonly User Default = new User(Guid.Empty);
    private string _name;
    private string _email;
    private string _phoneNumber;
    private UserType _userType;

    public User(Guid id, string name, string email, string phoneNumber, UserType userType) : base(id)
    {
        _name = name;
        _email = email;
        _phoneNumber = phoneNumber;
        _userType = userType;
    }

    public User(Guid id) : base(id)
    {
    }
    public string Name => _name;
    public string Email => _email;
    public string PhoneNumber => _phoneNumber;
    public UserType UserType => _userType;
    public Order? CurrentOrder { get; private set; }
    public Guid? CurrentOrderId { get; private set; }

    public User UpdateCurrentOrder(Order? currentOrder)
    {
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
}
