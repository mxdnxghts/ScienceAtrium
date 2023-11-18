using MediatR;
using ScienceAtrium.Domain.OrderAggregate;

namespace ScienceAtrium.Application.OrderAggregate.Commands.Handlers;
public class UpdateOrderHandler(IOrderRepository<Order> _orderRepository) : IRequestHandler<UpdateOrderCommand, int>
{
    public async Task<int> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        return await _orderRepository.UpdateAsync(request.Order, cancellationToken);
    }
}
