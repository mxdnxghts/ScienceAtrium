using MediatR;
using ScienceAtrium.Application.OrderAggregate.Queries;
using ScienceAtrium.Application.UserAggregate.CustomerAggregate.Commands;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Domain.RootAggregate.Options;
using ScienceAtrium.Infrastructure.Data;

namespace ScienceAtrium.Application.OrderAggregate.Commands.Handlers;
public class PayOrderHandler(ApplicationTransactionService _applicationTransactionService, IMediator _mediator) : IRequestHandler<PayOrderCommand, bool>
{
    public async Task<bool> Handle(PayOrderCommand request, CancellationToken cancellationToken)
    {
        request.Order.UpdateOrderDate(request.OrderDate);
        if (!request.Order.IsReadyToPay)
            return false;

        request.Order.UpdateStatus(OrderStatus.PaymentWait);

        await _applicationTransactionService
            .BeginTransactionScopeAsync(async () =>
            {
                await _mediator.Send(new UpdateOrderCommand(request.Order), cancellationToken);
                await _mediator.Send(new UpdateOrderWorkTemplateAfterPayingCommand(request.Order.Id), cancellationToken);
            }, cancellationToken: cancellationToken);

        var updatedOrder = await _mediator.Send(
            new GetOrderQuery(
                new EntityFindOptions<Order>(predicate: x => x.Id == request.Order.Id)),
            cancellationToken);

        await _mediator.Send(new MoveUnpaidWorkTemplatesCommand(updatedOrder), cancellationToken);

        await _mediator.Send(new UpdateCachedCustomerCommand(request.Customer), cancellationToken);

        return true;
    }
}
