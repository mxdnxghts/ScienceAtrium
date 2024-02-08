using MediatR;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;

namespace ScienceAtrium.Application.OrderAggregate.Commands;
public record PayOrderCommand(Customer Customer, Order Order, DateTime OrderDate) : IRequest;
