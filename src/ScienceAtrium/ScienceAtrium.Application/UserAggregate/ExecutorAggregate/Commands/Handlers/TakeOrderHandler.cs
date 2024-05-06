using System.Data;
using MediatR;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Domain.UserAggregate.ExecutorAggregate;
using ScienceAtrium.Infrastructure.Data;

namespace ScienceAtrium.Application.UserAggregate.ExecutorAggregate.Commands.Handlers;
public class TakeOrderHandler(IOrderRepository<Order> _orderRepository,
                              IUserRepository<Executor> _executorRepository,
                              ApplicationTransactionService _transactionService) : IRequestHandler<TakeOrderCommand, int>
{
    public async Task<int> Handle(TakeOrderCommand request, CancellationToken cancellationToken)
    {
        if (request is null || request.Executor is null || request.Order is null)
            return -1;
        if (!request.Executor.IsValid(_executorRepository) || !request.Order.IsValid(_orderRepository))
            return -1;

		request.Executor.AddOrder(request.Order);
		request.Order.UpdateExecutor(_executorRepository, request.Executor);

		await _transactionService.BeginTransactionScopeAsync(async () =>
        {
            await _executorRepository.UpdateAsync(request.Executor, cancellationToken);
            await _orderRepository.UpdateAsync(request.Order, cancellationToken);
        }, IsolationLevel.RepeatableRead, cancellationToken);

        return 0;
    }
}
