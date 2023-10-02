using ScienceAtrium.Domain.Entities;
using ScienceAtrium.Domain.WorkTemplateAggregate;

public class WorkTemplate : Entity
{
    public WorkTemplate(Guid id) : base(id)
    {
    }
    public string Title { get; set; }
    public string Description { get; set; }
    public WorkType WorkType { get; init; }
    public decimal Price { get; private set; }
}
