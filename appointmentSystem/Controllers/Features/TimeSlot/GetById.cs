using appointmentSystem.Data;
using appointmentSystem.Exceptions;
using appointmentSystem.Models.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace appointmentSystem.Controllers.Features.TimeSlot;

[ApiController]
[ApiExplorerSettings(GroupName = "TimeSlot")]
public class GetTimeSlotByIdController : ControllerBase
{
    private readonly IMediator _mediator;

    public GetTimeSlotByIdController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("api/TimeSlot/{id}")]
    public async Task<IActionResult> GetTimeSlot(Guid id)
    {
        var timeSlotModel = await _mediator.Send(new GetTimeSlotByIdQuery(id));

        return Ok(timeSlotModel);
    }
    
    public record GetTimeSlotByIdQuery(Guid Id) : IRequest<TimeSlotViewModel>;
    
    public class GetTimeSlotByIdQueryHandler : IRequestHandler<GetTimeSlotByIdQuery, TimeSlotViewModel>
    {
        private readonly AppDbContext _dbContext;

        public GetTimeSlotByIdQueryHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    
        public async Task<TimeSlotViewModel> Handle(GetTimeSlotByIdQuery request, CancellationToken cancellationToken)
        {
            var timeSlot = await _dbContext.TimeSlots.FindAsync(request.Id);

            if (timeSlot == null)
            {
                throw new NotFoundException("TimeSlot is not found");
            }
            
            return new TimeSlotViewModel(timeSlot.Id, timeSlot.StartTime, timeSlot.EndTime);
        }
    }
}