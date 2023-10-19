using ScienceAtrium.Domain.Entities;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;
using ScienceAtrium.Domain.WorkTemplateAggregate;
using System.ComponentModel.DataAnnotations.Schema;
using ScienceAtrium.Domain.UserAggregate.ExecutorAggregate;

namespace ScienceAtrium.Domain.OrderAggregate;

public class Order : Entity
{
    [NotMapped]
    public static readonly Order Default = new (Guid.Empty);
    public Order(Guid id) : base(id)
    {
    }
    public DateTime OrderDate { get; } = DateTime.UtcNow;
    public decimal TotalPrice
    {
        get => WorkTemplates.Sum(x => x.Price);
        private set
        {
        }
    }
    public Paymentmethod PaymentMethod { get; init; } = Paymentmethod.YooMoney;
    public Status Status { get; set; } = Status.Pending;
    public Customer? Customer { get; init; }
    public Guid? CustomerId { get; set; }
    public Executor? Executor { get; init; }
    public Guid? ExecutorId { get; set; }
    public List<WorkTemplate> WorkTemplates { get; private set; } = new();
}