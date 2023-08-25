using System.ComponentModel.DataAnnotations;
using appointmentSystem.Data;
using appointmentSystem.Models.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace appointmentSystem.Controllers.Features.TimeSlot;

[ApiController]
[ApiExplorerSettings(GroupName = "TimeSlot")]
public class CreateTimeSlotController : ControllerBase
{
    private readonly IMediator _mediator;

    public CreateTimeSlotController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("api/TimeSlot")]
    public async Task<TimeSlotViewModel> CreateService(CreateTimeSlotCommand command)
    {
        var result = await _mediator.Send(command);

        return result;
    }

    public record CreateTimeSlotCommand : IRequest<TimeSlotViewModel>
    {
        [Required] public DateTime StartTime { get; set; }
        [Required] public DateTime EndTime { get; set; }
        [Required] public Guid ServiceId { get; set; }
    };
    
    public class CreateTimeSlotCommandHandler : IRequestHandler<CreateTimeSlotCommand, TimeSlotViewModel>
    {
        private readonly AppDbContext _dbContext;

        public CreateTimeSlotCommandHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TimeSlotViewModel> Handle(CreateTimeSlotCommand request, CancellationToken cancellationToken)
        {
            var service = await _dbContext.Services.FindAsync(request.ServiceId);
            if (service is null)
            {
                throw new InvalidOperationException("This service is not found");
            }
            var endTime = request.StartTime.AddMinutes(service.DurationInMinutes);
            var hasOverlappingTimeSlot = await _dbContext.TimeSlots
                .AnyAsync(x => x.EndTime >= request.StartTime && x.StartTime <= endTime, cancellationToken);
    
            if (hasOverlappingTimeSlot)
            {
                throw new InvalidOperationException($"A time slot within the requested time range already exists.");
            }
            
            var timeSlot = new Models.TimeSlot
            {
                StartTime = request.StartTime,
                EndTime = endTime,
            };

            await _dbContext.TimeSlots.AddAsync(timeSlot, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new TimeSlotViewModel(timeSlot.Id, timeSlot.StartTime, timeSlot.EndTime);
        }
    }
}