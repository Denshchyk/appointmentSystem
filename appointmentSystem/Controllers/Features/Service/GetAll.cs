using appointmentSystem.Data;
using appointmentSystem.Exceptions;
using appointmentSystem.Models.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace appointmentSystem.Controllers.Features.Service;

[ApiController]
[ApiExplorerSettings(GroupName = "Services")]
public class ServiceController : ControllerBase
{
    private readonly IMediator _mediator;

    public ServiceController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("api/services")]
    public async Task<IActionResult> GetAllServices()
    {
        var services = await _mediator.Send(new Service.ServiceController.GetAllServicesQuery());

        return Ok(services);
    }
    
    public record GetAllServicesQuery : IRequest<List<ServiceViewModel>>;
    
    public class GetAllServicesQueryHandler : IRequestHandler<Service.ServiceController.GetAllServicesQuery, List<ServiceViewModel>>
    {
        private readonly AppDbContext _dbContext;

        public GetAllServicesQueryHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    
        public async Task<List<ServiceViewModel>> Handle(Service.ServiceController.GetAllServicesQuery request, CancellationToken cancellationToken)
        {
            var service = await _dbContext.Services.ToListAsync(cancellationToken);
            
            if (service is null)
            {
                throw new NotFoundException("Services are not found");
            }

            return service.Select(service => new ServiceViewModel(service.Id, service.Name, service.Description, service.DurationInMinutes, service.Price)).ToList();
        }
    }
}
