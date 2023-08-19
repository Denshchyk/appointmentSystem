using System;
using System.Threading;
using System.Threading.Tasks;
using appointmentSystem.Data;
using appointmentSystem.Exceptions;
using appointmentSystem.Models.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace appointmentSystem.Controllers.Features.Service;

[ApiController]
[ApiExplorerSettings(GroupName = "Services")]
public class GetServiceByIdController : ControllerBase
{
    private readonly IMediator _mediator;

    public GetServiceByIdController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("api/services/{id}")]
    public async Task<IActionResult> GetService(Guid id)
    {
        var serviceModel = await _mediator.Send(new GetServiceByIdQuery(id));

        return Ok(serviceModel);
    }
    
    public record GetServiceByIdQuery(Guid Id) : IRequest<ServiceViewModel>;
    
    public class GetServiceByIdQueryHandler : IRequestHandler<GetServiceByIdQuery, ServiceViewModel>
    {
        private readonly AppDbContext _dbContext;

        public GetServiceByIdQueryHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    
        public async Task<ServiceViewModel> Handle(GetServiceByIdQuery request, CancellationToken cancellationToken)
        {
            var service = await _dbContext.Services.FindAsync(request.Id);

            if (service == null)
            {
                throw new NotFoundException("Service name is not found");
            }
            
            return new ServiceViewModel(service.Id, service.Name, service.Description, service.DurationInMinutes, service.Price);
        }
    }
}