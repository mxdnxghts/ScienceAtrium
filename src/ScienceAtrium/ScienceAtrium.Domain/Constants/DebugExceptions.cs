namespace ScienceAtrium.Domain.Constants;

public static class DebugExceptions
{
	public static string NullOrWhiteSpace(string instance)
	{
		return $"Passed {instance} is null, empty or consists only of white-space characters";
	}

	public static string HasIncorrectValue(string instance)
	{
		return $"Passed instance of {{{instance}}} has incorrect value.";
	}

	public static string EntityWithSameKey(string instance, Guid key)
	{
		return $"{instance} with the same Key {{{key}}} already exists.";
	}

	public static string HasNullValue(string instance)
	{
		return $"{instance} is null";
	}
}