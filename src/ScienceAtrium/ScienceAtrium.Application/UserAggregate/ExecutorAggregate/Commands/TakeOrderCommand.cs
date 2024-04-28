using MediatR;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Domain.UserAggregate.ExecutorAggregate;

namespace ScienceAtrium.Application.UserAggregate.ExecutorAggregate.Commands;
public record TakeOrderCommand(Executor Executor, Order Order) : IRequest;
