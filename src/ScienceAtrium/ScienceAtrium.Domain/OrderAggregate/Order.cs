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

    private List<WorkTemplate> _workTemplates;
    private Status _status;
    private Paymentmethod _paymentMethod;
    private decimal _totalCost;

    public Order(Guid id) : base(id)
    {
        _workTemplates = new();
        _status = Status.Pending;
        _paymentMethod = Paymentmethod.YooMoney;
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

    public void AddWorkTemplate(WorkTemplate workTemplate)
    {
        if (workTemplate is null || workTemplate.IsEmpty())
        {
            Debug.Fail($"Passed instance of {{{nameof(WorkTemplate)}}} has incorrect value.");
            return;
        }

        var existingWorkTemplate = _workTemplates.FirstOrDefault(x => x.Id == workTemplate.Id);
        if (existingWorkTemplate?.IsEmpty() == true)
        {
            Debug.Fail($"Work template with the same Key {{{existingWorkTemplate.Id}}} already exists.");
            return;
        }
        _workTemplates.Add(workTemplate);
        _totalCost = _workTemplates.Sum(x => x.Price);
    }

    public void UpdateStatus(Status status)
    {
        _status = status;
    }

    public void UpdateCustomer(IReader<Customer> reader, Customer customer)
    {
        if (!customer.IsValid(reader))
        {
            Debug.Fail($"Passed instance of {{{nameof(Customer)}}} has incorrect value.");
            return;
        }

        Customer = customer;
        CustomerId = customer.Id;
    }

    public void UpdateExecutor(IReader<Executor> reader, Executor executor)
    {
        if (!executor.IsValid(reader))
        {
            Debug.Fail($"Passed instance of {{{nameof(Customer)}}} has incorrect value.");
            return;
        }

        Executor = executor;
        ExecutorId = executor.Id;
    }
}