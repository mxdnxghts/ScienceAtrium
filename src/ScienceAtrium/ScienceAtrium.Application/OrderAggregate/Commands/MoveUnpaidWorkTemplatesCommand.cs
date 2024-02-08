using MediatR;
using ScienceAtrium.Domain.OrderAggregate;

namespace ScienceAtrium.Application.OrderAggregate.Commands;
public record MoveUnpaidWorkTemplatesCommand(Order Order) : IRequest;
