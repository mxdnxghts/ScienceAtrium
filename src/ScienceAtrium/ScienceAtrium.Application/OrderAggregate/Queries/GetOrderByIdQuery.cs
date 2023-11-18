using MediatR;
using ScienceAtrium.Domain.OrderAggregate;

namespace ScienceAtrium.Application.OrderAggregate.Queries;
public record GetOrderByIdQuery(Guid Id) : IRequest<Order>;
