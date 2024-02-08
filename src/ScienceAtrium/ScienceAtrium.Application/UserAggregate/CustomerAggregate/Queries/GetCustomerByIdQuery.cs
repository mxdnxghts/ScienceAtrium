using MediatR;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;

namespace ScienceAtrium.Application.UserAggregate.CustomerAggregate.Queries;
public record GetCustomerByIdQuery(Guid CustomerId) : IRequest<Customer>;
