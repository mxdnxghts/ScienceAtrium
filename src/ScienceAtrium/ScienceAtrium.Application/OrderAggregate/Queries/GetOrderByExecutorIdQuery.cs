using MediatR;
using ScienceAtrium.Domain.OrderAggregate;

namespace ScienceAtrium.Application.OrderAggregate.Queries;
public record GetOrderByExecutorIdQuery(Guid ExecutorId) : IRequest<Order>;
