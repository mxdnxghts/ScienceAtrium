using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Domain.UserAggregate;

namespace Domain.UnitTests;

public class TestManageUserOrders
{
    [Test]
    public void TestAddOrder()
    {
        var order = new Order(Guid.NewGuid());
        var customer = new User(Guid.NewGuid(), UserType.Customer).AddOrder(order);
        var executor = new User(Guid.NewGuid(), UserType.Executor).AddOrder(order);
        Assert.That(customer.Orders, Has.Count.AtLeast(0));
        Assert.That(executor.Orders, Has.Count.AtLeast(0));
    }

    [Test]
    public void TestUpdateOrder()
    {
        var order = new Order(Guid.NewGuid()).UpdateStatus(OrderStatus.Pending);
        var customer = new User(Guid.NewGuid(), UserType.Customer).AddOrder(order);
        var executor = new User(Guid.NewGuid(), UserType.Executor).AddOrder(order);

        customer.UpdateOrder(o => o.Status == OrderStatus.Pending, new Order(order.Id).UpdateStatus(OrderStatus.Cancelled));
        executor.UpdateOrder(o => o.Status == OrderStatus.Pending, new Order(order.Id).UpdateStatus(OrderStatus.Cancelled));

        Assert.That(customer.Orders.FirstOrDefault().Status, Is.EqualTo(OrderStatus.Cancelled));
        Assert.That(executor.Orders.FirstOrDefault().Status, Is.EqualTo(OrderStatus.Cancelled));
    }

    [Test]
    public void TestDeleteOrder()
    {
        var order = new Order(Guid.NewGuid()).UpdateStatus(OrderStatus.Pending);
        var customer = new User(Guid.NewGuid(), UserType.Customer).AddOrder(order);
        var executor = new User(Guid.NewGuid(), UserType.Executor).AddOrder(order);

        customer.RemoveOrder(o => o.Status == OrderStatus.Pending);
        executor.RemoveOrder(o => o.Status == OrderStatus.Pending);

        Assert.That(customer.Orders.FirstOrDefault(), Is.Null);
        Assert.That(executor.Orders.FirstOrDefault(), Is.Null);
    }
}
