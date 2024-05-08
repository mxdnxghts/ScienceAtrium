using MediatR;
using ScienceAtrium.Domain.RootAggregate.Options;
using ScienceAtrium.Domain.UserAggregate.ExecutorAggregate;

namespace ScienceAtrium.Application.UserAggregate.ExecutorAggregate.Queries;
public record GetExecutorQuery(EntityFindOptions<Executor> EntityFindOptions) : IRequest<Executor>;
