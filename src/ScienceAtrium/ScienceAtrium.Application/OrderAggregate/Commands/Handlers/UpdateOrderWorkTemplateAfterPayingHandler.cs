using MediatR;
using Microsoft.EntityFrameworkCore;
using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Domain.OrderAggregate;
using ScienceAtrium.Infrastructure.Extensions;

namespace ScienceAtrium.Application.OrderAggregate.Commands.Handlers;
public class UpdateOrderWorkTemplateAfterPayingHandler(ApplicationContext _context, Serilog.ILogger _logger) : IRequestHandler<UpdateOrderWorkTemplateAfterPayingCommand, int>
{
    public async Task<int> Handle(UpdateOrderWorkTemplateAfterPayingCommand request, CancellationToken cancellationToken)
    {
        var orderWorkTemplates = await _context
            .OrderWorkTemplates
            .Where(x => x.OrderId == request.OrderId)
            .ToListAsync(cancellationToken: cancellationToken);

        foreach (var orderWorkTemplate in orderWorkTemplates)
            orderWorkTemplate.State = WorkTemplateState.Paid;

        _context.OrderWorkTemplates.UpdateRange(orderWorkTemplates);
        return await _context.TrySaveChangesAsync(_logger, cancellationToken: cancellationToken);
    }
}
