using appointmentSystem.Data;
using appointmentSystem.Exceptions;
using appointmentSystem.Models.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace appointmentSystem.Controllers.Features.Service;

[ApiController]
[ApiExplorerSettings(GroupName = "Services")]
public class GetServiceByNameController : ControllerBase
{
    private readonly IMediator _mediator;

    public GetServiceByNameController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("api/services/{name}")]
    public async Task<IActionResult> GetService(string name)
    {
        var serviceModel = await _mediator.Send(new GetServiceByNameQuery(name));

        return Ok(serviceModel);
    }
    
    public record GetServiceByNameQuery(string name) : IRequest<ServiceViewModel>;
    
    public class GetServiceByNameQueryHandler : IRequestHandler<GetServiceByNameQuery, ServiceViewModel>
    {
        private readonly AppDbContext _dbContext;

        public GetServiceByNameQueryHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    
        public async Task<ServiceViewModel> Handle(GetServiceByNameQuery request, CancellationToken cancellationToken)
        {
            var service = await _dbContext.Services.FindAsync(request.name);

            if (service == null)
            {
                throw new NotFoundException("Service name is not found");
            }
            
            return new ServiceViewModel(service.Id, service.Name, service.Description, service.DurationInMinutes, service.Price);
        }
    }
}