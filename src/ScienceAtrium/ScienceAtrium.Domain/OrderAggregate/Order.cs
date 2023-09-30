using ScienceAtrium.Domain.Entities;

public class Order : Entity
{
    public Order(Guid id) : base(id)
    {
    }
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public Paymentmethod PaymentMethod { get; set; }
    public Status Status { get; set; }
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