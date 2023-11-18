using MediatR;
using ScienceAtrium.Domain.OrderAggregate;

namespace ScienceAtrium.Application.OrderAggregate.Queries;
public record GetOrderByCustomerIdQuery(Guid CustomerId) : IRequest<Order>;
