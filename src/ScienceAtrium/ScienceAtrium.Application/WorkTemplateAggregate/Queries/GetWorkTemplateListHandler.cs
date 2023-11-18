using MediatR;
using ScienceAtrium.Domain.WorkTemplateAggregate;

namespace ScienceAtrium.Application.WorkTemplateAggregate.Queries;
public class GetWorkTemplateListHandler(IWorkTemplateRepository<WorkTemplate> _workTemplateRepository)
	: IRequestHandler<GetWorkTemplateListQuery, List<WorkTemplate>>
{
	public Task<List<WorkTemplate>> Handle(GetWorkTemplateListQuery request, CancellationToken cancellationToken)
	{
		return Task.FromResult(_workTemplateRepository.All.ToList());
	}
}
