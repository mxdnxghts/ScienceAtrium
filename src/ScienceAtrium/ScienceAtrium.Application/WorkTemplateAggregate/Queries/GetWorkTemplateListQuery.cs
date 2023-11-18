using MediatR;
using ScienceAtrium.Domain.WorkTemplateAggregate;

namespace ScienceAtrium.Application.WorkTemplateAggregate.Queries;
public record GetWorkTemplateListQuery() : IRequest<List<WorkTemplate>>;