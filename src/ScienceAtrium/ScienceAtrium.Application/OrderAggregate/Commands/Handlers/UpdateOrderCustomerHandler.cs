using MediatR;
using ScienceAtrium.Application.OrderAggregate.Queries;
using ScienceAtrium.Domain.RootAggregate.Interfaces;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;

namespace ScienceAtrium.Application.OrderAggregate.Commands.Handlers;
public class UpdateOrderCustomerHandler(IMediator _mediator, IReader<Customer> _customerReader) : IRequestHandler<UpdateOrderCustomerCommand, int>
{
    public async Task<int> Handle(UpdateOrderCustomerCommand request, CancellationToken cancellationToken)
    {
        var order = await _mediator.Send(new GetOrderByIdQuery(request.OrderId), cancellationToken);
        return await _mediator.Send(new UpdateOrderCommand(order.UpdateCustomer(_customerReader, request.Customer)), cancellationToken);
    }
}
