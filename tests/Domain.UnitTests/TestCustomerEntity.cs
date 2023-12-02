using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Domain.UserAggregate;

namespace Domain.UnitTests;

public class TestCustomerEntity
{
	[SetUp]
	public void Setup()
	{
	}

	[Test]
	public void UpdateCurrentOrderTest()
	{
		var customer = new User(Guid.NewGuid(), UserType.Customer);
		var order = new Order(Guid.NewGuid()).UpdateStatus(Status.Fulfilled);
		customer.AddOrder(order);
		customer.UpdateOrder(x => x.Id == order.Id, new Order(Guid.Empty));

		Assert.That(customer.Orders.FirstOrDefault(x => x.Id == order.Id), Is.EqualTo(order));
	}
}
