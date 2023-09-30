using System;

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
