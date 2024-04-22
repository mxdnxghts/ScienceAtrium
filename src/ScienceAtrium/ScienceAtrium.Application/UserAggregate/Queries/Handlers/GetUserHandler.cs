using AutoMapper;
using MediatR;
using ScienceAtrium.Domain.RootAggregate.Interfaces;
using ScienceAtrium.Domain.UserAggregate;

namespace ScienceAtrium.Application.UserAggregate.Queries.Handlers;
public class GetUserHandler<TUser>(IMapper mapper,
                                   IReaderAsync<TUser> _reader)
    : IRequestHandler<GetUserQuery<TUser>, TUser> where TUser : User
{
    public async Task<TUser> Handle(GetUserQuery<TUser> request, CancellationToken cancellationToken)
    {
        if (request is null || request.EntityFindOptions is null)
            return mapper.Map<TUser>(User.Default);

        var user = await _reader.GetAsync(request.EntityFindOptions, cancellationToken);
        return user;
    }
}
