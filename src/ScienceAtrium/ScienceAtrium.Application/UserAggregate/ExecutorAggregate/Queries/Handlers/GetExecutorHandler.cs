using AutoMapper;
using MediatR;
using ScienceAtrium.Domain.RootAggregate.Interfaces;
using ScienceAtrium.Domain.UserAggregate;
using ScienceAtrium.Domain.UserAggregate.ExecutorAggregate;

namespace ScienceAtrium.Application.UserAggregate.ExecutorAggregate.Queries.Handlers;
public class GetExecutorHandler(IMapper mapper, IReaderAsync<Executor> _reader) : IRequestHandler<GetExecutorQuery, Executor>
{
    public async Task<Executor> Handle(GetExecutorQuery request, CancellationToken cancellationToken)
    {
        if (request is null || request.EntityFindOptions is null)
            return mapper.Map<Executor>(User.Default);

        var user = await _reader.GetAsync(request.EntityFindOptions, cancellationToken);
        return user;
    }
}
