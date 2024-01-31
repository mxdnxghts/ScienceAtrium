using MediatR;
using ScienceAtrium.Application.OrderAggregate.Queries;
using ScienceAtrium.Domain.OrderAggregate;

namespace ScienceAtrium.Application.OrderAggregate.Queries.Handlers;
public class GetOrderByCustomerIdWithStatusHandler(IOrderRepository<Order> _orderRepository)
    : IRequestHandler<GetOrderByCustomerIdWithStatusQuery, Order>
{
    public async Task<Order> Handle(GetOrderByCustomerIdWithStatusQuery request, CancellationToken cancellationToken)
    {
        return await _orderRepository.GetAsync(x => x.CustomerId == request.CustomerId
                                            && x.Status == request.OrderStatus, cancellationToken);
    }
}
