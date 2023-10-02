using ScienceAtrium.Domain.Entities;
using ScienceAtrium.Domain.UserAggregate;

public class Order : Entity
{
    public Order(Guid id) : base(id)
    {
    }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public decimal TotalPrice { get; set; }
    public Paymentmethod PaymentMethod { get; set; }
    public Status Status { get; set; }
    public User Customer { get; set; }
    public User Executor { get; set; }
    public List<WorkTemplate> WorkTemplates { get; set; } = new();
}
public enum Paymentmethod
{
    YooMoney
}
public enum Status
{
    Pending,
    Fulfilled,
    Cancelled,
    Delayed,
    Expired
}