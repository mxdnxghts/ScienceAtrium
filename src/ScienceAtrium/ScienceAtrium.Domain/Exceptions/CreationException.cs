namespace ScienceAtrium.Domain.Exceptions;

public class CreationException : Exception
{
	public CreationException() : base()
	{
	}

	public CreationException(Guid entityId) : base($"You're trying add entity with the same Id: {{{entityId}}}")
	{
	}

	public CreationException(string? message) : base(message)
	{
	}

	public CreationException(string? message, Exception? innerException) : base(message, innerException)
	{
	}
}