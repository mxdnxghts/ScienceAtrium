using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;
using ScienceAtrium.Domain.UserAggregate.ExecutorAggregate;

namespace Domain.UnitTests;

public class TestManageUserOrders
{
    [Test]
    public void TestAddOrder()
    {
        var order = new Order(Guid.NewGuid());
        var customer = new Customer(Guid.NewGuid()).AddFormedOrder(order);
        var executor = new Executor(Guid.NewGuid()).AddDoneOrder(order);
        Assert.That(customer.FormedOrders, Has.Count.AtLeast(0));
        Assert.That(executor.DoneOrders, Has.Count.AtLeast(0));
    }

    [Test]
    public void TestUpdateOrder()
    {
        var order = new Order(Guid.NewGuid()).UpdateStatus(Status.Pending);
        var customer = new Customer(Guid.NewGuid()).AddFormedOrder(order);
        var executor = new Executor(Guid.NewGuid()).AddDoneOrder(order);

        customer.UpdateFormedOrder(o => o.Status == Status.Pending, new Order(order.Id).UpdateStatus(Status.Cancelled));
        executor.UpdateDoneOrder(o => o.Status == Status.Pending, new Order(order.Id).UpdateStatus(Status.Cancelled));

        Assert.That(customer.FormedOrders.FirstOrDefault().Status, Is.EqualTo(Status.Cancelled));
        Assert.That(executor.DoneOrders.FirstOrDefault().Status, Is.EqualTo(Status.Cancelled));
    }

    [Test]
    public void TestDeleteOrder()
    {
        var order = new Order(Guid.NewGuid()).UpdateStatus(Status.Pending);
        var customer = new Customer(Guid.NewGuid()).AddFormedOrder(order);
        var executor = new Executor(Guid.NewGuid()).AddDoneOrder(order);

        customer.RemoveFormedOrder(o => o.Status == Status.Pending);
        executor.RemoveDoneOrder(o => o.Status == Status.Pending);

        Assert.That(customer.FormedOrders.FirstOrDefault(), Is.Null);
        Assert.That(executor.DoneOrders.FirstOrDefault(), Is.Null);
    }
}
