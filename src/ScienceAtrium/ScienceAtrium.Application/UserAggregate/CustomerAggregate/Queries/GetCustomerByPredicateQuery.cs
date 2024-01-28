using MediatR;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;
using System.Linq.Expressions;

namespace ScienceAtrium.Application.UserAggregate.CustomerAggregate.Queries;
public record GetCustomerByPredicateQuery(Expression<Func<Customer, bool>> Predicate) : IRequest<Customer>;
