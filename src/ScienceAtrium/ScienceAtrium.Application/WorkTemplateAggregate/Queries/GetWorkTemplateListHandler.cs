using MediatR;
using Microsoft.EntityFrameworkCore;
using ScienceAtrium.Domain.WorkTemplateAggregate;
using ScienceAtrium.Infrastructure.Data;

namespace ScienceAtrium.Application.WorkTemplateAggregate.Queries;
public class GetWorkTemplateListHandler(ApplicationContext _context)
	: IRequestHandler<GetWorkTemplateListQuery, List<WorkTemplate>>
{
	public async Task<List<WorkTemplate>> Handle(GetWorkTemplateListQuery request, CancellationToken cancellationToken)
	{
		return await _context.WorkTemplates.Include(x => x.Subject).ToListAsync();
	}
}
