using MediatR;
using ScienceAtrium.Domain.OrderAggregate;

namespace ScienceAtrium.Application.OrderAggregate.Commands;
public record UpdateOrderCommand(Order Order) : IRequest<int>;