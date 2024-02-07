using MediatR;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;
using System.Linq.Expressions;

namespace ScienceAtrium.Application.UserAggregate.CustomerAggregate.Queries;
public record GetCustomerQuery(Guid? CustomerId = null, Expression<Func<Customer, bool>>? Predicate = null) : IRequest<Customer>;
