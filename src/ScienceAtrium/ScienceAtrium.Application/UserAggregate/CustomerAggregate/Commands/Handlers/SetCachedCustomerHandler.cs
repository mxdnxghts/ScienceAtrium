using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using ScienceAtrium.Application.Extensions;
using ScienceAtrium.Domain.RootAggregate.Interfaces;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;

namespace ScienceAtrium.Application.UserAggregate.CustomerAggregate.Commands.Handlers;
public class SetCachedCustomerHandler(IDistributedCache _cache, IReader<Customer> _customerReader)
    : IRequestHandler<SetCachedCustomerCommand>
{
    public async Task Handle(SetCachedCustomerCommand request, CancellationToken cancellationToken)
    {
        if (!request.Customer.IsValid(_customerReader))
            return;

        await _cache.SetRecordAsync("CachedCustomer", request.Customer, TimeSpan.FromMinutes(10), cancellationToken: cancellationToken);
    }
}
