using MediatR;
using ScienceAtrium.Domain.OrderAggregate;

namespace ScienceAtrium.Application.OrderAggregate.Commands;
public record CreateOrderCommand(Order Order) : IRequest<int>;