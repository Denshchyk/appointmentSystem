using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using appointmentSystem.Data;
using appointmentSystem.Models;
using appointmentSystem.Models.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace appointmentSystem.Controllers.Features.Service;

[ApiController]
[ApiExplorerSettings(GroupName = "Services")]
public class CreateServiceController : ControllerBase
{
    private readonly IMediator _mediator;

    public CreateServiceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("api/services")]
    public async Task<ServiceViewModel> CreateService(CreateServiceCommand command)
    {
        var result = await _mediator.Send(command);

        return result;
    }

    public record CreateServiceCommand : IRequest<ServiceViewModel>
    {
        [Required] [StringLength(20)] public string Name { get; set; }
        [Required] [StringLength(100)] public string Description { get; set; }
        [Required] public int DurationInMinutes { get; set; }
        [Required] public decimal Price { get; set; }
    };
    
    public class CreateServiceCommandHandler : IRequestHandler<CreateServiceCommand, ServiceViewModel>
    {
        private readonly AppDbContext _dbContext;

        public CreateServiceCommandHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ServiceViewModel> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
        {
            var existingService = await _dbContext.Services.FirstOrDefaultAsync(s => s.Name == request.Name, cancellationToken);

            if(existingService != null)
            {
                throw new InvalidOperationException($"A service with the name {request.Name} already exists.");
            }
            var service = new Models.Service
            {
                Name = request.Name,
                Description = request.Description,
                DurationInMinutes = request.DurationInMinutes,
                Price = request.Price
            };

            await _dbContext.Services.AddAsync(service, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new ServiceViewModel(service.Id, service.Name, service.Description, service.DurationInMinutes, service.Price);
        }
    }
}