using System.ComponentModel.DataAnnotations;
using appointmentSystem.Data;
using appointmentSystem.Exceptions;
using appointmentSystem.Models.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace appointmentSystem.Controllers.Features.TimeSlot;

[ApiController]
[ApiExplorerSettings(GroupName = "TimeSlot")]
public class UpdateTimeSlotController : ControllerBase
{
    private readonly IMediator _mediator;

    public UpdateTimeSlotController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPut("api/TimeSlot/{id}")]
    public async Task<IActionResult> UpdateTimeSlot(Guid id, UpdateTimeSlotViewModel updateTimeSlotViewModel)
    {
        var updateTimeSlotCommand = new UpdateTimeSlotCommand (id, updateTimeSlotViewModel.StartTime,  updateTimeSlotViewModel.EndTime);
        var result = await _mediator.Send(updateTimeSlotCommand);

        return Ok(result);
    }

    public record UpdateTimeSlotViewModel
    {
        [Required] public Guid Id { get; set; }
        [Required] public DateTime StartTime { get; set; }
        [Required] public DateTime EndTime { get; set; }
        [Required] public Guid ServiceId { get; set; }
    };
    

    public record UpdateTimeSlotCommand(Guid Id, DateTime StartTime, DateTime EndTime) : IRequest<TimeSlotViewModel>;

    public class UpdateTimeSlotCommandHandler : IRequestHandler<UpdateTimeSlotCommand, TimeSlotViewModel>
    {
        private readonly AppDbContext _dbContext;

        public UpdateTimeSlotCommandHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TimeSlotViewModel> Handle(UpdateTimeSlotCommand request, CancellationToken cancellationToken)
        {
            var service = await _dbContext.Services.FindAsync(request.Id);
            var timeSlot = await _dbContext.TimeSlots.FindAsync(request.Id);
            

            if (timeSlot is null)
            {
                throw new NotFoundException("TimeSlot is not found");
            }

            timeSlot.StartTime = request.StartTime;
            timeSlot.EndTime = request.StartTime.AddMinutes(service.DurationInMinutes);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new TimeSlotViewModel(timeSlot.Id, timeSlot.StartTime, timeSlot.EndTime);
        }
    }
}