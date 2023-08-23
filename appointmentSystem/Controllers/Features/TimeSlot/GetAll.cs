using appointmentSystem.Data;
using appointmentSystem.Exceptions;
using appointmentSystem.Models.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace appointmentSystem.Controllers.Features.TimeSlot;

[ApiController]
[ApiExplorerSettings(GroupName = "TimeSlot")]
public class TimeSlotController : ControllerBase
{
    private readonly IMediator _mediator;

    public TimeSlotController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("api/TimeSlot")]
    public async Task<IActionResult> GetAllTimeSlots()
    {
        var timeSlots = await _mediator.Send(new GetAllTimeSlotsQuery());

        return Ok(timeSlots);
    }
    
    public record GetAllTimeSlotsQuery : IRequest<List<TimeSlotViewModel>>;
    
    public class GetAllTimeSlotsQueryHandler : IRequestHandler<GetAllTimeSlotsQuery, List<TimeSlotViewModel>>
    {
        private readonly AppDbContext _dbContext;

        public GetAllTimeSlotsQueryHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    
        public async Task<List<TimeSlotViewModel>> Handle(GetAllTimeSlotsQuery request, CancellationToken cancellationToken)
        {
            var timeSlot = await _dbContext.TimeSlots.ToListAsync(cancellationToken);
            
            if (timeSlot is null)
            {
                throw new NotFoundException("TimeSlots are not found");
            }

            return timeSlot.Select(ts => new TimeSlotViewModel(ts.Id, ts.StartTime, ts.EndTime)).ToList();
        }
    }
}
