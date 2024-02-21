using MediatR;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;

namespace ScienceAtrium.Application.UserAggregate.CustomerAggregate.Commands.Handlers;
public class CreateCustomerHandler(IUserRepository<Customer> _customerRepository) : IRequestHandler<CreateCustomerCommand, int>
{
    public async Task<int> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        return await _customerRepository.CreateAsync(request.Customer, cancellationToken);
    }
}
