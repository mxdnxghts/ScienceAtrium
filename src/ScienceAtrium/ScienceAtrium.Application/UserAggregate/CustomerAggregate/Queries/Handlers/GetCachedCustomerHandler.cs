using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using ScienceAtrium.Application.Extensions;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;

namespace ScienceAtrium.Application.UserAggregate.CustomerAggregate.Queries.Handlers;
public class GetCachedCustomerHandler(IDistributedCache _cache, IMapper _mapper) : IRequestHandler<GetCachedCustomerQuery, Customer>
{
    public async Task<Customer> Handle(GetCachedCustomerQuery request, CancellationToken cancellationToken)
    {
        var customerJson = await _cache.GetRecordAsync<CustomerJson>("CachedCustomer", cancellationToken: cancellationToken);
		return _mapper.Map<Customer>(customerJson);
    }
}
