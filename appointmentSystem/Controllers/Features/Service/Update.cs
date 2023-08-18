using System.ComponentModel.DataAnnotations;
using appointmentSystem.Data;
using appointmentSystem.Exceptions;
using appointmentSystem.Models.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace appointmentSystem.Controllers.Features.Service;

[ApiController]
[ApiExplorerSettings(GroupName = "Services")]
public class UpdateServiceController : ControllerBase
{
    private readonly IMediator _mediator;

    public UpdateServiceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPut("api/services/{id}")]
    public async Task<IActionResult> UpdateService(Guid id, UpdateServiceViewModel updateServiceViewModel)
    {
        var updateServiceCommand = new UpdateServiceCommand (id, updateServiceViewModel.Name, updateServiceViewModel.Duration, updateServiceViewModel.Price);
        var result = await _mediator.Send(updateServiceCommand);

        return Ok(result);
    }

    public record UpdateServiceViewModel
    {
        [Required] [StringLength(20)] public string Name { get; set; }
        [Required] [CreateServiceController.MaxDurationAttribute] public TimeSpan Duration { get; set; }
        [Required] [Range(0, 1000)] public decimal Price { get; set; }
    };
    

    public record UpdateServiceCommand(Guid Id, string Name, TimeSpan Duration, decimal Price) : IRequest<ServiceViewModel>;

    public class UpdateServiceCommandHandler : IRequestHandler<Service.UpdateServiceController.UpdateServiceCommand, ServiceViewModel>
    {
        private readonly AppDbContext _dbContext;

        public UpdateServiceCommandHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ServiceViewModel> Handle(Service.UpdateServiceController.UpdateServiceCommand request, CancellationToken cancellationToken)
        {
            var service = await _dbContext.Services.FindAsync(request.Id);

            if (service is null)
            {
                throw new NotFoundException("Service is not found");
            }

            service.Name = request.Name;
            service.Duration = request.Duration;
            service.Price = request.Price;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new ServiceViewModel(service.Id, service.Name, service.Duration, service.Price);
        }
    }
}