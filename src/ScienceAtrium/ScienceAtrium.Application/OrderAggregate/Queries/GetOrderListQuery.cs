using MediatR;
using ScienceAtrium.Domain.OrderAggregate;

namespace ScienceAtrium.Application.OrderAggregate.Queries;
public record GetOrderListQuery() : IRequest<List<Order>>;
