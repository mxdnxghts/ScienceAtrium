using MediatR;

namespace ScienceAtrium.Application.OrderAggregate.Commands;
public record DeleteOrderCommand(Guid OrderId) : IRequest<int>;