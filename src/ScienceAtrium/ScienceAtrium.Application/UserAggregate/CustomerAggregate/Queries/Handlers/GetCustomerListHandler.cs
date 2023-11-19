using MediatR;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;

namespace ScienceAtrium.Application.UserAggregate.CustomerAggregate.Queries.Handlers;
public class GetCustomerListHandler(IUserRepository<Customer> _customerRepository) : IRequestHandler<GetCustomerListQuery, List<Customer>>
{
    public Task<List<Customer>> Handle(GetCustomerListQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_customerRepository.All.ToList());
    }
}
