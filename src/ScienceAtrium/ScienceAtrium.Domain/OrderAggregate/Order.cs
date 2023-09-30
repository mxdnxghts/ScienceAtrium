public class Order
{
    public Guid Id { get; set; }
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