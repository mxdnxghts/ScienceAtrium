namespace ScienceAtrium.Domain.Exceptions;
public class EntityNotFoundException : Exception
{
    public EntityNotFoundException()
        : base("Entity wasn't found in the database.")
    {
    }

    public EntityNotFoundException(string? message) : base(message)
    {
    }

    public EntityNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
