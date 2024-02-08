using MediatR;
using ScienceAtrium.Domain.OrderAggregate;

namespace ScienceAtrium.Application.OrderAggregate.Queries.Handlers;
public class GetOrderHandler(IOrderRepository<Order> _orderRepository) : IRequestHandler<GetOrderQuery, Order>
{
	public async Task<Order> Handle(GetOrderQuery request, CancellationToken cancellationToken)
	{
		return await _orderRepository.GetAsync(request.entityFindOptions, cancellationToken);
	}
}
