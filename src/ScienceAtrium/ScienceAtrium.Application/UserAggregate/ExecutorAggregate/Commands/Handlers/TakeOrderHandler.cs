using MediatR;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Domain.UserAggregate.ExecutorAggregate;

namespace ScienceAtrium.Application.UserAggregate.ExecutorAggregate.Commands.Handlers;
public class TakeOrderHandler(IOrderRepository<Order> _orderRepository,
                              IUserRepository<Executor> _executorRepository) : IRequestHandler<TakeOrderCommand>
{
    public async Task Handle(TakeOrderCommand request, CancellationToken cancellationToken)
    {
        if (request is null || request.Executor is null || request.Order is null)
            return;
        if (!request.Executor.IsValid(_executorRepository) || !request.Order.IsValid(_orderRepository))
            return;


    }
}
