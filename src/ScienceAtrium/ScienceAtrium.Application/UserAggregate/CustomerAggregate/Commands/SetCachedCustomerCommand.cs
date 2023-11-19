using MediatR;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;

namespace ScienceAtrium.Application.UserAggregate.CustomerAggregate.Commands;
public record SetCachedCustomerCommand(Customer Customer) : IRequest;
