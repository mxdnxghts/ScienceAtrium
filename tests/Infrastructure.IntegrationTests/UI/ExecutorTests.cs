using Infrastructure.IntegrationTests.Extensions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ScienceAtrium.Application.OrderAggregate.Commands;
using ScienceAtrium.Application.UserAggregate.CustomerAggregate.Queries;
using ScienceAtrium.Application.UserAggregate.ExecutorAggregate.Commands;
using ScienceAtrium.Application.UserAggregate.ExecutorAggregate.Queries;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Domain.RootAggregate.Options;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;
using ScienceAtrium.Domain.UserAggregate.ExecutorAggregate;

namespace Infrastructure.IntegrationTests.UI;


public class ExecutorTests
{
	[SetUp]
	public void SetUp()
	{
		Setup.StartUp();
	}
	
	[TearDown]
	public void TearDown()
	{
		Setup.TurnOff();
	}

	[Test]
	public async Task TakeOrderTest()
	{
		var mediator = Setup.Provider.GetRequiredService<IMediator>();
		var customerReader = Setup.Provider.GetRequiredService<IUserRepository<Customer>>();
		
		var notValidExecutor = TestExtension.GetNewExecutor();

		var validExecutor = await mediator.Send(
			 new GetExecutorQuery(
				  new EntityFindOptions<Executor>(
					  predicate: x => x.Id != Guid.Empty)));
		var validCustomer = await mediator.Send(
			 new GetCustomerQuery(
				  new EntityFindOptions<Customer>(
					  predicate: x => x.Id != Guid.Empty)));

		var order = new Order(Guid.NewGuid())
			.UpdateCustomer(customerReader, validCustomer)
			.UpdateOrderDate(DateTime.Now.AddDays(7))
			.UpdateStatus(OrderStatus.Pending);

		var createOrder = await mediator.Send(new CreateOrderCommand(order));
		int failedTake;
		var goodTake = await mediator.Send(new TakeOrderCommand(validExecutor, order));
		try
		{
			failedTake = await mediator.Send(new TakeOrderCommand(notValidExecutor, order));
		}
		catch (Exception)
		{
			failedTake = -1;
		}

		Assert.Multiple(() =>
		{
			Assert.That(notValidExecutor, Is.Not.Null);
			Assert.That(validExecutor, Is.Not.Null);
			Assert.That(validCustomer, Is.Not.Null);
			Assert.That(order, Is.Not.Null);
		});

		Assert.Multiple(() =>
		{
			Assert.That(createOrder, Is.AtLeast(0));
			Assert.That(failedTake, Is.EqualTo(-1));
			Assert.That(goodTake, Is.EqualTo(0));
		});
	}
}
