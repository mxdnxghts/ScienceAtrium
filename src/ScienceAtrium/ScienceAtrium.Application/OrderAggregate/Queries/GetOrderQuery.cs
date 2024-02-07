using MediatR;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Domain.RootAggregate.Options;

namespace ScienceAtrium.Application.OrderAggregate.Queries;
public record GetOrderQuery(EntityFindOptions<Order> entityFindOptions) : IRequest<Order>;
