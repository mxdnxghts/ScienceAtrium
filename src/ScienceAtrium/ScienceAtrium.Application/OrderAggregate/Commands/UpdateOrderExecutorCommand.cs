using MediatR;
using ScienceAtrium.Domain.UserAggregate.ExecutorAggregate;

namespace ScienceAtrium.Application.OrderAggregate.Commands;
public record UpdateOrderExecutorCommand(Guid OrderId, Executor Executor) : IRequest<int>;