using MediatR;
using ScienceAtrium.Domain.OrderAggregate;

namespace ScienceAtrium.Application.OrderAggregate.Queries;
public record GetOrderByCustomerIdWithStatusQuery(Guid CustomerId, OrderStatus OrderStatus) : IRequest<Order>;