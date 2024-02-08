using Newtonsoft.Json;
using ScienceAtrium.Domain.OrderAggregate;

namespace ScienceAtrium.Domain.UserAggregate.ExecutorAggregate;
public class Executor : User
{
    public Executor(Guid id) : base(id, UserType.Executor)
    {
    }

    [JsonConstructor]
    public Executor(
        Guid id,
        string name,
        string email,
        string phoneNumber,
        List<Order> orders)
        : base(id, name, email, phoneNumber, UserType.Executor, orders)
    {
    }
}
