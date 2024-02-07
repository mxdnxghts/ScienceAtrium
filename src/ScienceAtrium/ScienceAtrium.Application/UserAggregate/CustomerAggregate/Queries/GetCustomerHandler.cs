using MediatR;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;

namespace ScienceAtrium.Application.UserAggregate.CustomerAggregate.Queries;
public class GetCustomerHandler(IUserRepository<Customer> _userRepository) : IRequestHandler<GetCustomerQuery, Customer>
{
	public async Task<Customer> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
	{
		return await _userRepository.GetAsync(request.CustomerId, request.Predicate, cancellationToken);
	}
}
