using MediatR;
using ScienceAtrium.Domain.OrderAggregate;

namespace ScienceAtrium.Application.OrderAggregate.Queries.Handlers;
public class GetOrderListHandler(IOrderRepository<Order> _orderRepository) : IRequestHandler<GetOrderListQuery, List<Order>>
{
    public Task<List<Order>> Handle(GetOrderListQuery request, CancellationToken cancellationToken)
    {
        if (request.WhereExpression is not null)
            return Task.FromResult(_orderRepository.All.Where(request.WhereExpression).ToList());
		return Task.FromResult(_orderRepository.All.ToList());
	}
}
