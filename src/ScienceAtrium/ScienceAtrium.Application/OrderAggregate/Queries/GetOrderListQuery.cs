using MediatR;
using ScienceAtrium.Domain.OrderAggregate;
using System.Linq.Expressions;

namespace ScienceAtrium.Application.OrderAggregate.Queries;
public record GetOrderListQuery(Expression<Func<Order, bool>>? WhereExpression = null) : IRequest<List<Order>>;
