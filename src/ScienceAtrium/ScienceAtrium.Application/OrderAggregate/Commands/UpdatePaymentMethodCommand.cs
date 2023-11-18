using MediatR;
using ScienceAtrium.Domain.OrderAggregate;

namespace ScienceAtrium.Application.OrderAggregate.Commands;
public record UpdatePaymentMethodCommand(Guid OrderId, Paymentmethod Paymentmethod) : IRequest<int>;