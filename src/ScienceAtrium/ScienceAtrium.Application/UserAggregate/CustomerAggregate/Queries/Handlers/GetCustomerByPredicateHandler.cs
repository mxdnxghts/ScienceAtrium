using MediatR;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;

namespace ScienceAtrium.Application.UserAggregate.CustomerAggregate.Queries.Handlers;
public class GetCustomerByPredicateHandler(IUserRepository<Customer> _userRepository)
    : IRequestHandler<GetCustomerByPredicateQuery, Customer>
{
    public async Task<Customer> Handle(GetCustomerByPredicateQuery request, CancellationToken cancellationToken)
    {
        return await _userRepository.GetAsync(predicate: request.Predicate, cancellationToken: cancellationToken);
    }
}
