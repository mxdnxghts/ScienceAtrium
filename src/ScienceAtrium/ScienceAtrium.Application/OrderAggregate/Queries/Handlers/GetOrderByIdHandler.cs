using MediatR;
using ScienceAtrium.Domain.OrderAggregate;

namespace ScienceAtrium.Application.OrderAggregate.Queries.Handlers;
public class GetOrderByIdHandler(IMediator _mediator) : IRequestHandler<GetOrderByIdQuery, Order>
{
    public async Task<Order> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var orders = await _mediator.Send(new GetOrderListQuery(), cancellationToken);
        return orders.Find(x => x.Id == request.Id) ?? Order.Default;
    }
}
