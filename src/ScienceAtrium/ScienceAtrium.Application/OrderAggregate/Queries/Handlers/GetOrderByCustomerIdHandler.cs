using MediatR;
using ScienceAtrium.Domain.OrderAggregate;

namespace ScienceAtrium.Application.OrderAggregate.Queries.Handlers;
public class GetOrderByCustomerIdHandler(IMediator _mediator) : IRequestHandler<GetOrderByCustomerIdQuery, Order>
{
    public async Task<Order> Handle(GetOrderByCustomerIdQuery request, CancellationToken cancellationToken)
    {
        var orders = await _mediator.Send(new GetOrderListQuery());
        return orders.Find(x => x.Customer?.Id == request.CustomerId) ?? Order.Default;
    }
}
