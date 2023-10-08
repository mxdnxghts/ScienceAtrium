using ScienceAtrium.Domain.Entities;

namespace ScienceAtrium.Domain.WorkTemplateAggregate;

public class WorkTemplate : Entity
{
    public object Customer;
    public object Executor;

    public WorkTemplate(Guid id) : base(id)
    {
    }
    public string Title { get; set; }
    public string Description { get; set; }
    public WorkType WorkType { get; init; }
    public decimal Price { get; init; }
}