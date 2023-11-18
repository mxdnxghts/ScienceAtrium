using ScienceAtrium.Domain.Constants;
using ScienceAtrium.Domain.RootAggregate;
using ScienceAtrium.Domain.RootAggregate.Interfaces;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;
using ScienceAtrium.Domain.UserAggregate.ExecutorAggregate;
using ScienceAtrium.Domain.WorkTemplateAggregate;
using System.Diagnostics;

namespace ScienceAtrium.Domain.OrderAggregate;

public class Order : Entity
{
    public static readonly Order Default = new(Guid.Empty);

    private readonly List<WorkTemplate> _workTemplates;
    private Status _status;
    private Paymentmethod _paymentMethod;
    private decimal _totalCost;

    public Order(Guid id) : base(id)
    {
        _workTemplates = new();
        _status = Status.Pending;
        _paymentMethod = Paymentmethod.YooMoney;
    }

    public Order(Guid id, Status status, Paymentmethod paymentMethod) : base(id)
    {
        _workTemplates = new();
        _status = status;
        _paymentMethod = paymentMethod;
    }
    public DateTime OrderDate { get; } = DateTime.UtcNow;
    public decimal TotalCost => _totalCost;
    public Paymentmethod PaymentMethod => _paymentMethod;
    public Status Status => _status;
    public Customer? Customer { get; private set; }
    public Guid? CustomerId { get; private set; }
    public Executor? Executor { get; private set; }
    public Guid? ExecutorId { get; private set; }
    public IReadOnlyCollection<WorkTemplate> WorkTemplates => _workTemplates;

    public Order AddWorkTemplate(WorkTemplate workTemplate)
    {
        if (!ThrowIfHasIncorrectValue(workTemplate))
            return this;
        _workTemplates.Add(workTemplate);
        _totalCost = _workTemplates.Sum(x => x.Price);
        return this;
    }

    public Order UpdateWorkTemplate(WorkTemplate workTemplate)
    {
        if (!ThrowIfHasIncorrectValue(workTemplate))
            return this;
        _workTemplates.Remove(workTemplate);
        _workTemplates.Add(workTemplate);
        _totalCost = _workTemplates.Sum(x => x.Price);
        return this;
    }

    public Order RemoveWorkTemplate(Func<WorkTemplate, bool> funcGetWorkTemplate)
    {
        var workTemplate = _workTemplates.FirstOrDefault(funcGetWorkTemplate);
        if (!ThrowIfHasIncorrectValue(workTemplate))
            return this;
        _workTemplates.Remove(workTemplate);
        _totalCost = _workTemplates.Sum(x => x.Price);
        return this;
    }

    public Order UpdateStatus(Status status)
    {
        _status = status;
        return this;
    }

    public Order UpdatePaymentMethod(Paymentmethod paymentMethod)
    {
        _paymentMethod = paymentMethod;
        return this;
    }

    public Order UpdateCustomer(IReader<Customer> reader, Customer customer)
    {
        if (!customer.IsValid(reader))
        {
            Debug.Fail(DebugExceptions.HasIncorrectValue(nameof(Customer)));
            return this;
        }

        Customer = customer;
        CustomerId = customer.Id;
        return this;
    }

    public Order UpdateExecutor(IReader<Executor> reader, Executor executor)
    {
        if (!executor.IsValid(reader))
        {
            Debug.Fail(DebugExceptions.HasIncorrectValue(nameof(Customer)));
            return this;
        }

        Executor = executor;
        ExecutorId = executor.Id;
        return this;
    }

    private bool ThrowIfHasIncorrectValue(WorkTemplate workTemplate)
    {
        if (workTemplate is null || workTemplate.IsEmpty())
        {
            Debug.Fail(DebugExceptions.HasIncorrectValue(nameof(WorkTemplate)));
            return false;
        }

        var existingWorkTemplate = _workTemplates.Find(x => x.Id == workTemplate.Id);
        if (existingWorkTemplate?.IsEmpty() == true)
        {
            Debug.Fail(DebugExceptions.EntityWithSameKey(nameof(WorkTemplate), existingWorkTemplate.Id));
            return false;
        }
        return true;
    }
}