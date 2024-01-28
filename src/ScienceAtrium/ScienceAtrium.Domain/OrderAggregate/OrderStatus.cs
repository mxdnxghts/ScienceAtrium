namespace ScienceAtrium.Domain.OrderAggregate;
public enum OrderStatus
{
    Delayed,
    PaymentWait,
    Pending,
    Fulfilled,
    Cancelled,
    Expired
}
