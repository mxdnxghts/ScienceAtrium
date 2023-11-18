using MediatR;
using ScienceAtrium.Domain.OrderAggregate;

namespace ScienceAtrium.Application.OrderAggregate.Commands.Handlers;
public class DeleteOrderHandler(IOrderRepository<Order> _orderRepository) : IRequestHandler<DeleteOrderCommand, int>
{
    public async Task<int> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        return await _orderRepository.DeleteAsync(await _orderRepository.GetAsync(x => x.Id == request.OrderId, cancellationToken), cancellationToken);
    }
}
