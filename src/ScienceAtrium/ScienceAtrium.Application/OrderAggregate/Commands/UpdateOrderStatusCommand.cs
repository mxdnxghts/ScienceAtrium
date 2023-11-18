using MediatR;
using ScienceAtrium.Domain.OrderAggregate;

namespace ScienceAtrium.Application.OrderAggregate.Commands;
public record UpdateOrderStatusCommand(Guid OrderId, Status NewStatus) : IRequest<int>;
