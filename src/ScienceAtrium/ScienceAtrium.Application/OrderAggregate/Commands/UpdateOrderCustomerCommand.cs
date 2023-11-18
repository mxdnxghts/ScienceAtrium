using MediatR;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;

namespace ScienceAtrium.Application.OrderAggregate.Commands;
public record UpdateOrderCustomerCommand(Guid OrderId, Customer Customer) : IRequest<int>;
