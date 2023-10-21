using FluentValidation.Results;
using ScienceAtrium.Domain.Exceptions;

namespace ScienceAtrium.Domain.Exceptions;
public class ValidationException : Exception
{
    public ValidationException()
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(Guid entityId) : base($"Entity with Id {{{entityId}}} doesn't fit conditions.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }

    public ValidationException(string? message) : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(string? message, Exception? innerException) : base(message, innerException)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public IDictionary<string, string[]> Errors { get; }
}
