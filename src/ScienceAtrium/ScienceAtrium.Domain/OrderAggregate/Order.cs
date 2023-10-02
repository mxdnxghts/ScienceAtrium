using ScienceAtrium.Domain.Entities;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Domain.UserAggregate;

public class Order : Entity
{
    public Order(Guid id) : base(id)
    {
    }
    public DateTime OrderDate { get; } = DateTime.UtcNow;
    public decimal TotalPrice 
    {
        get => WorkTemplates.Sum(x => x.Price);
    }
    public Paymentmethod PaymentMethod { get; init; } = Paymentmethod.YooMoney;
    public Status Status { get; set; }
    public User? Customer { get; init; }
    public Guid? CustomerId { get; set; }
    public User? Executor { get; init; }
    public Guid? ExecutorId { get; set; }
    public List<WorkTemplate> WorkTemplates { get; private set; } = new();
}
