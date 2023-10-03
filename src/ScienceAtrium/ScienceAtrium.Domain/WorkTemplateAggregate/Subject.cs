using ScienceAtrium.Domain.Entities;
namespace ScienceAtrium.Domain.WorkTemplateAggregate;

public class Subject : Entity
{
    public Subject(Guid id) : base(id)
    {
    }
    public string Name { get; init; }
}

