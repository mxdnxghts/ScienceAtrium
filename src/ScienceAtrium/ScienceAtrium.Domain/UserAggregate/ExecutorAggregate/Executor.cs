namespace ScienceAtrium.Domain.UserAggregate.ExecutorAggregate;
public class Executor : User
{
    public Executor(Guid id) : base(id, UserType.Executor)
    {
    }
}
