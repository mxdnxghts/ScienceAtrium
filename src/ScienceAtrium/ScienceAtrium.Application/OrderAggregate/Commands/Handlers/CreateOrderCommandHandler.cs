using MediatR;
using ScienceAtrium.Domain.OrderAggregate;

namespace ScienceAtrium.Application.OrderAggregate.Commands.Handlers;
public class CreateOrderCommandHandler(IOrderRepository<Order> _orderRepository) : IRequestHandler<CreateOrderCommand, int>
{
    public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        return await _orderRepository.CreateAsync(request.Order, cancellationToken);
    }
}
