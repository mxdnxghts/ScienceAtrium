using MediatR;
using ScienceAtrium.Domain.OrderAggregate;

namespace ScienceAtrium.Application.OrderAggregate.Queries.Handlers;
public class GetOrderListHandler(IOrderRepository<Order> _orderRepository) : IRequestHandler<GetOrderListQuery, List<Order>>
{
    public Task<List<Order>> Handle(GetOrderListQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_orderRepository.All.ToList());
    }
}
