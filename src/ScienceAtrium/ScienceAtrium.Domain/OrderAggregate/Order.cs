using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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

    private readonly List<OrderWorkTemplate> _workTemplatesLink;
    private OrderStatus _status;
    private Paymentmethod _paymentMethod;
    private decimal _totalCost;
    private DateTime _orderDate = DateTime.UtcNow;

    public Order(Guid id) : base(id)
    {
		_workTemplatesLink = new();
        _status = OrderStatus.Delayed;
        _paymentMethod = Paymentmethod.YooMoney;
    }

    public Order(Guid id, OrderStatus status, Paymentmethod paymentMethod) : base(id)
    {
		_workTemplatesLink = new();
		_status = status;
        _paymentMethod = paymentMethod;
    }

    [JsonConstructor]
    protected Order(
        Guid id,
        List<OrderWorkTemplate> workTemplatesLink,
        OrderStatus status,
        Paymentmethod paymentMethod,
        decimal totalCost,
        DateTime orderDate,
        Customer? customer,
        Guid? customerId,
        Executor? executor,
        Guid? executorId)
        : base(id)
    {
        _workTemplatesLink = workTemplatesLink;
        _status = status;
        _paymentMethod = paymentMethod;
        _totalCost = totalCost;
        _orderDate = orderDate;
        Customer = customer;
        CustomerId = customerId;
        Executor = executor;
        ExecutorId = executorId;
    }

    public DateTime OrderDate => _orderDate;
    public decimal TotalCost => _totalCost;
    public Paymentmethod PaymentMethod => _paymentMethod;
    public OrderStatus Status => _status;
    public Customer? Customer { get; private set; }
    public Guid? CustomerId { get; private set; }
    public Executor? Executor { get; private set; }
    public Guid? ExecutorId { get; private set; }
    public IReadOnlyCollection<WorkTemplate> WorkTemplates => GetWorkTemplates();
    public IReadOnlyCollection<OrderWorkTemplate> WorkTemplatesLink => _workTemplatesLink;
    public bool IsReadyToPay =>
        WorkTemplates?.Count > 1
        && TotalCost > 0
        && OrderDate >= DateTime.UtcNow;

	public Order AddWorkTemplate(WorkTemplate workTemplate)
    {
        if (ThrowIfWorkTemplateHasIncorrectValue(workTemplate))
            return this;

        var workTemplateLink = _workTemplatesLink.FirstOrDefault(x => x.WorkTemplateId == workTemplate.Id);
        if (workTemplateLink is null)
        {
            _workTemplatesLink.Add(new OrderWorkTemplate(this, workTemplate)
            {
                State = WorkTemplateState.WaitInBasket,
                EntityState = EntityState.Added,
            });
        }
        else
            workTemplateLink.EntityState = EntityState.Modified;

        _totalCost = GetTotal();
        return this;
    }

    public Order AddWorkTemplates(params WorkTemplate[] workTemplates)
    {
        foreach (var workTemplate in workTemplates)
            AddWorkTemplate(workTemplate);

        return this;
    }

    public Order RemoveWorkTemplate(Func<WorkTemplate, bool> funcGetWorkTemplate)
    {
        var workTemplate = _workTemplatesLink
            .Select(x => x.WorkTemplate)
            .FirstOrDefault(funcGetWorkTemplate);

        if (ThrowIfWorkTemplateHasIncorrectValue(workTemplate))
            return this;

        _workTemplatesLink
            .FirstOrDefault(x => x.WorkTemplateId == workTemplate.Id).EntityState = EntityState.Deleted;

        _totalCost = GetTotal();
        return this;
    }

    public Order UpdateOrderDate(DateTime orderDate)
    {
        if (orderDate <= DateTime.UtcNow)
            return this;
        _orderDate = orderDate;
        return this;
    }

    public Order UpdateStatus(OrderStatus status)
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

    public Order RemoveCustomer()
	{
		Customer = null;
        CustomerId = null;
		return this;
	}

    public Order UpdateExecutor(IReader<Executor> reader, Executor executor)
    {
        if (executor?.IsValid(reader) != true)
            return this;

        Executor = executor;
        ExecutorId = executor.Id;
        return this;
    }

	public Order RemoveExecutor()
	{
		Executor = null;
		ExecutorId = null;
		return this;
    }

    private IReadOnlyCollection<WorkTemplate> GetWorkTemplates()
    {
        return _workTemplatesLink
            .Where(x => x.EntityState != EntityState.Deleted)
            .Select(x => x.WorkTemplate)
            .ToList();
    }

	private bool ThrowIfWorkTemplateHasIncorrectValue(WorkTemplate workTemplate)
    {
        if (workTemplate is null || workTemplate.IsEmpty())
        {
            Debug.Print(DebugExceptions.HasIncorrectValue(nameof(WorkTemplate)));
            return true;
        }

        var existingWorkTemplate = WorkTemplates.FirstOrDefault(x => x.Id == workTemplate.Id);
        if (existingWorkTemplate?.IsEmpty() == true)
        {
            Debug.Fail(DebugExceptions.EntityWithSameKey(nameof(WorkTemplate), existingWorkTemplate.Id));
            return true;
        }

        return false;
    }

    private decimal GetTotal()
    {
        return WorkTemplates.Sum(x => x.Price);
	}
}