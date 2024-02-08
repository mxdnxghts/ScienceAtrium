using MediatR;
using ScienceAtrium.Application.UserAggregate.CustomerAggregate.Commands;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Infrastructure.Data;

namespace ScienceAtrium.Application.OrderAggregate.Commands;
public class PayOrderHandler(ApplicationTransactionService _applicationTransactionService, IMediator _mediator) : IRequestHandler<PayOrderCommand>
{
	public async Task Handle(PayOrderCommand request, CancellationToken cancellationToken)
	{
		request.Order.UpdateOrderDate(request.OrderDate);
		if (!request.Order.IsReadyToPay)
			return;

		request.Order.UpdateStatus(OrderStatus.PaymentWait);

		await _applicationTransactionService
			.BeginTransactionScopeAsync(async () =>
			{
				await _mediator.Send(new UpdateOrderCommand(request.Order), cancellationToken);
				await _mediator.Send(new UpdateOrderWorkTemplateAfterPayingCommand(request.Order.Id), cancellationToken);
			}, cancellationToken: cancellationToken);
		
		await _mediator.Send(new SetCachedCustomerCommand(request.Customer), cancellationToken);
	}
}
