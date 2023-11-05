using AutoMapper;
using ScienceAtrium.Domain.OrderAggregate;
using System.ComponentModel.DataAnnotations.Schema;
using ScienceAtrium.Domain.RootAggregate;

namespace ScienceAtrium.Domain.UserAggregate;

public class User : Entity
{
    [NotMapped]
    public static readonly User Default = new User(Guid.Empty);
    public User(Guid id) : base(id)
    {
    }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public UserType UserType { get; set; }
    public Order? CurrentOrder { get; private set; }
    public Guid? CurrentOrderId { get; private set; }

    public User UpdateCurrentOrder(Order? currentOrder)
    {
        CurrentOrder = currentOrder;
        CurrentOrderId = currentOrder?.Id;
        return this;
    }

    public TUser MapTo<TUser>() where TUser : User
    {
        return new MapperConfiguration(mc => mc.CreateMap<User, TUser>())
            .CreateMapper()
            .Map<TUser>(this);
    }
}
