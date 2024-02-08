using MediatR;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Domain.RootAggregate.Interfaces;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;
using ScienceAtrium.Infrastructure.Data;

namespace ScienceAtrium.Application.OrderAggregate.Commands.Handlers;
public class MoveUnpaidWorkTemplatesHandler(
    IOrderRepository<Order> _orderRepository,
    IReader<Customer> _reader,
    ApplicationTransactionService _applicationTransactionService)
    : IRequestHandler<MoveUnpaidWorkTemplatesCommand>
{
    public async Task Handle(MoveUnpaidWorkTemplatesCommand request, CancellationToken cancellationToken)
    {
        var removeWorkTemplates = request.Order.WorkTemplatesLink
            .Where(x => x.Status == OrderWorkTemplateStatus.WaitInBasket)
            .Select(x => x.WorkTemplate)
            .ToArray();

        request.Order.RemoveWorkTemplates(removeWorkTemplates);

        var order = new Order(Guid.NewGuid())
            .UpdateCustomer(_reader, request.Order.Customer)
            .AddWorkTemplates(removeWorkTemplates);

        await _applicationTransactionService.BeginTransactionScopeAsync(async () =>
        {
            await _orderRepository.CreateAsync(order, cancellationToken);
            await _orderRepository.UpdateAsync(request.Order, cancellationToken);
        }, cancellationToken: cancellationToken);
    }
}
