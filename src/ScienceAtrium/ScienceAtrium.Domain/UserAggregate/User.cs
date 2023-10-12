using ScienceAtrium.Domain.Entities;
using ScienceAtrium.Domain.OrderAggregate;
using System.ComponentModel.DataAnnotations.Schema;

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
    public Order? CurrentOrder { get; set; }
    public Guid? CurrentOrderId { get; set; }

    public void UpdateDetails(Order order)
    {
        CurrentOrder = order;
        CurrentOrderId = order.Id;
    }
}
