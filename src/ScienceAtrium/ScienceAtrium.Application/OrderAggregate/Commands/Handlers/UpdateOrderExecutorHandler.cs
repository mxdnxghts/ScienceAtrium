using MediatR;
using ScienceAtrium.Application.OrderAggregate.Queries;
using ScienceAtrium.Domain.RootAggregate.Interfaces;
using ScienceAtrium.Domain.UserAggregate.ExecutorAggregate;

namespace ScienceAtrium.Application.OrderAggregate.Commands.Handlers;
public class UpdateOrderExecutorHandler(IMediator _mediator, IReader<Executor> _executorReader) : IRequestHandler<UpdateOrderExecutorCommand, int>
{
    public async Task<int> Handle(UpdateOrderExecutorCommand request, CancellationToken cancellationToken)
    {
        var order = await _mediator.Send(new GetOrderByIdQuery(request.OrderId), cancellationToken);
        return await _mediator.Send(new UpdateOrderCommand(order.UpdateExecutor(_executorReader, request.Executor)), cancellationToken);
    }
}
