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
}
