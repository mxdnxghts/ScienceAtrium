using MediatR;
using ScienceAtrium.Domain.OrderAggregate;

namespace ScienceAtrium.Application.OrderAggregate.Queries.Handlers;
public class GetOrderByExecutorIdHandler(IMediator _mediator) : IRequestHandler<GetOrderByExecutorIdQuery, Order>
{
    public async Task<Order> Handle(GetOrderByExecutorIdQuery request, CancellationToken cancellationToken)
    {
        var orders = await _mediator.Send(new GetOrderListQuery());
        return orders.FirstOrDefault(x => x.Executor?.Id == request.ExecutorId) ?? Order.Default;
    }
}
