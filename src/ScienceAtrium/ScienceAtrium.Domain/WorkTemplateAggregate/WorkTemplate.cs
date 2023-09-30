using ScienceAtrium.Domain.Entities;

public class WorkTemplate : Entity
{
    public WorkTemplate(Guid id) : base(id)
    {
    }
    public string Title { get; set; }
    public string Description { get; set; }
    public WorkType WorkType { get; set; }
}
public enum WorkType
{
    CourseWork,
    LaboratoryWork,
    CustomWork
}
