using MediatR;
using ScienceAtrium.Application.OrderAggregate.Queries;

namespace ScienceAtrium.Application.OrderAggregate.Commands.Handlers;
public class UpdatePaymentMethodHandler(IMediator _mediator) : IRequestHandler<UpdatePaymentMethodCommand, int>
{
    public async Task<int> Handle(UpdatePaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var order = await _mediator.Send(new GetOrderByIdQuery(request.OrderId), cancellationToken);
        return await _mediator.Send(new UpdateOrderCommand(order.UpdatePaymentMethod(request.Paymentmethod)));
    }
}
