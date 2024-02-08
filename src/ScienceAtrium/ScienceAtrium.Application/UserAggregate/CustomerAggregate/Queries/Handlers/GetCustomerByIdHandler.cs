using MediatR;
using ScienceAtrium.Domain.RootAggregate.Options;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;

namespace ScienceAtrium.Application.UserAggregate.CustomerAggregate.Queries.Handlers;
public class GetCustomerByIdHandler(IUserRepository<Customer> _userRepository) 
    : IRequestHandler<GetCustomerByIdQuery, Customer>
{
    public async Task<Customer> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        return await _userRepository.GetAsync(new EntityFindOptions<Customer>(request.CustomerId), cancellationToken: cancellationToken);
    }
}
