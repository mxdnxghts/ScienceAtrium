namespace ScienceAtrium.Domain.WorkTemplateAggregate;
public interface IWorkTemplateRepository
{
    public class WorkTemplate
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public WorkType Worktype { get; set; }
    }
    public enum WorkType
    {
        CourseWork,
        LaboratoryWork,
        CustomWork
    }

}
