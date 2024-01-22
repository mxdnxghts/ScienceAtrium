using MediatR;

namespace ScienceAtrium.Application.OrderAggregate.Commands;
public record UpdateOrderWorkTemplateAfterPayingCommand(Guid OrderId) : IRequest<int>;
