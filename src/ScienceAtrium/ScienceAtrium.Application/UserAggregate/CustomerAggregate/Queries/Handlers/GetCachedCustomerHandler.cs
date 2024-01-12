using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using ScienceAtrium.Application.Extensions;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;

namespace ScienceAtrium.Application.UserAggregate.CustomerAggregate.Queries.Handlers;
public class GetCachedCustomerHandler(IDistributedCache _cache) : IRequestHandler<GetCachedCustomerQuery, Customer>
{
    public async Task<Customer> Handle(GetCachedCustomerQuery request, CancellationToken cancellationToken)
    {
        return await _cache.GetRecordAsync<Customer>("CachedCustomer", cancellationToken: cancellationToken);
	}
}
