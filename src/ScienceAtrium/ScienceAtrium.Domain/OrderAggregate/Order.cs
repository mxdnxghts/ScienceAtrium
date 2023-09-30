using System;

public class Order
{
    public Guid Id { get; set; }
    public date OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public Paymentmethod Paymentmethod { get; set; }
    public Status Status { get; set; }
    public List<WorkTemplate> WorkTemplates { get; set; }
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