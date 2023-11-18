using MediatR;
using ScienceAtrium.Domain.OrderAggregate;

namespace ScienceAtrium.Application.OrderAggregate.Commands.Handlers;
public class UpdateOrderStatusHandler(IOrderRepository<Order> _orderRepository) : IRequestHandler<UpdateOrderStatusCommand, int>
{
    public async Task<int> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetAsync(x => x.Id == request.OrderId);
        order?.UpdateStatus(request.NewStatus);
        return await _orderRepository.UpdateAsync(order, cancellationToken);
    }
}
