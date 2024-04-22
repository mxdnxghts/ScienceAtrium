using MediatR;
using ScienceAtrium.Domain.RootAggregate.Options;
using ScienceAtrium.Domain.UserAggregate;

namespace ScienceAtrium.Application.UserAggregate.Queries;
public record GetUserQuery<TUser>(EntityFindOptions<TUser> EntityFindOptions) : IRequest<TUser> where TUser : User;
