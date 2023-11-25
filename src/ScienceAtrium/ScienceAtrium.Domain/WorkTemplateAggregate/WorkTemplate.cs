using ScienceAtrium.Domain.Constants;
using ScienceAtrium.Domain.RootAggregate;
using System.Diagnostics;

namespace ScienceAtrium.Domain.WorkTemplateAggregate;

public class WorkTemplate : Entity
{
    public static readonly WorkTemplate Default = new(Guid.Empty);
    private string _title;
    private string _description;
    private WorkType _workType;
    private decimal _price;

    public WorkTemplate(Guid id) : base(id)
    {
        _title = string.Empty;
        _description = string.Empty;
    }
    //public WorkTemplate(Guid id, string title, string description, WorkType workType, decimal price) : base(id)
    //{
    //    _title = title;
    //    _description = description;
    //    _workType = workType;
    //    _price = price;
    //}
    public string Title => _title ?? string.Empty;
    public string Description => _description ?? string.Empty;
    public WorkType WorkType => _workType;
    public decimal Price => _price;
    public Guid? SubjectId { get; private set; }
    public Subject? Subject { get; private set; }

    public WorkTemplate UpdateSubject(Subject subject)
    {
        if (subject?.IsEmpty() == true)
        {
            Debug.Fail(DebugExceptions.HasIncorrectValue(nameof(subject)));
            return this;
        }

        Subject = subject;
        SubjectId = subject.Id;

        return this;
    }

    public WorkTemplate UpdateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            Debug.Fail(DebugExceptions.NullOrWhiteSpace(nameof(title)));
            return this;
        }

        _title = title;
        return this;
    }

    public WorkTemplate UpdateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            Debug.Fail(DebugExceptions.NullOrWhiteSpace(nameof(description)));
            return this;
        }

        _description = description;
        return this;
    }

    public WorkTemplate UpdateWorkType(WorkType workType)
    {
        _workType = workType;
        return this;
    }

    public WorkTemplate UpdatePrice(decimal price)
    {
        _price = price;
        return this;
    }
}