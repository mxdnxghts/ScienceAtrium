using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using ScienceAtrium.Domain.RootAggregate.Interfaces;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;
using ScienceAtrium.Infrastructure.Extensions;

namespace ScienceAtrium.Application.UserAggregate.CustomerAggregate.Commands.Handlers;
public class UpdateCachedCustomerHandler(IDistributedCache _cache, IReader<Customer> _customerReader)
	: IRequestHandler<UpdateCachedCustomerCommand>
{
	public async Task Handle(UpdateCachedCustomerCommand request, CancellationToken cancellationToken)
	{
		if (!request.Customer.IsValid(_customerReader))
			return;

        await _cache.SetRecordAsync($"UserCached_{request.Customer.Id}", request.Customer, cancellationToken: cancellationToken);
    }
}