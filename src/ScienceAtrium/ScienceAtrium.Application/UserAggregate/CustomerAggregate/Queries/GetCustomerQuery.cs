using MediatR;
using ScienceAtrium.Domain.RootAggregate.Options;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;

namespace ScienceAtrium.Application.UserAggregate.CustomerAggregate.Queries;
public record GetCustomerQuery(EntityFindOptions<Customer> EntityFindOptions) : IRequest<Customer>;
