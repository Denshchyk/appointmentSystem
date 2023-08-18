using System.ComponentModel.DataAnnotations;
using appointmentSystem.Data;
using appointmentSystem.Models;
using appointmentSystem.Models.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<ServiceViewModel> CreateService(Service.CreateServiceController.CreateServiceCommand command)
    {
        var result = await _mediator.Send(command);

        return result;
    }

    public record CreateServiceCommand : IRequest<ServiceViewModel>
    {
        [Required] [StringLength(20)] public string Name { get; set; }
        [Required] [MaxDuration] public TimeSpan Duration { get; set; }
        [Required] [Range(0, 1000)] public decimal Price { get; set; }
    };
    
    public class MaxDurationAttribute : ValidationAttribute
    {
        private readonly TimeSpan _maxDuration = TimeSpan.FromHours(24);

        public override bool IsValid(object value)
        {
            if (value is TimeSpan timeSpan)
            {
                return timeSpan <= _maxDuration;
            }

            return false;
        }
    }

    public class CreateServiceCommandHandler : IRequestHandler<Service.CreateServiceController.CreateServiceCommand, ServiceViewModel>
    {
        private readonly AppDbContext _dbContext;

        public CreateServiceCommandHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ServiceViewModel> Handle(Service.CreateServiceController.CreateServiceCommand request, CancellationToken cancellationToken)
        {
            var service = new Models.Service
            {
                Name = request.Name,
                Duration = request.Duration,
                Price = request.Price
            };

            await _dbContext.Services.AddAsync(service, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new ServiceViewModel(service.Id, service.Name, service.Duration, service.Price);
        }
    }
}