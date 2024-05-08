using Microsoft.EntityFrameworkCore;
using ScienceAtrium.Domain.WorkTemplateAggregate;

namespace ScienceAtrium.Domain.OrderAggregate;

public class OrderWorkTemplate
{
	public OrderWorkTemplate()
	{
	}

	public OrderWorkTemplate(Order order, WorkTemplate workTemplate)
	{
		OrderId = order.Id;
		Order = order;

		WorkTemplateId = workTemplate.Id;
		WorkTemplate = workTemplate;
	}

	public Guid? OrderId { get; set; }
	public Order Order { get; set; }
	public Guid? WorkTemplateId { get; set; }
	public WorkTemplate WorkTemplate { get; set; }
	public OrderWorkTemplateStatus Status { get; set; } = OrderWorkTemplateStatus.Delayed;
	public EntityState EntityState { get; set; } = EntityState.Detached;

	public override string ToString()
	{
		return $"{nameof(OrderId)}: {OrderId}\n{nameof(WorkTemplateId)}: {WorkTemplateId}\n{nameof(Status)}: {Status}";
	}
}