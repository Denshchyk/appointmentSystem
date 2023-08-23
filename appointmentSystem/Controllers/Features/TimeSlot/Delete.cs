using appointmentSystem.Data;
using appointmentSystem.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace appointmentSystem.Controllers.Features.TimeSlot;

[ApiController]
[ApiExplorerSettings(GroupName = "TimeSlot")]
public class DeleteTimeSlotController : ControllerBase
{
    private readonly IMediator _mediator;

    public DeleteTimeSlotController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpDelete("api/TimeSlot/{id}")]
    public async Task<IActionResult> DeleteTimeSlot(Guid id)
    {
        await _mediator.Send(new DeleteTimeSlotCommand(id));

        return NoContent();
    }

    public record DeleteTimeSlotCommand(Guid Id) : IRequest;

    public class DeleteTimeSlotCommandHandler : IRequestHandler<DeleteTimeSlotCommand>
    {
        private readonly AppDbContext _dbContext;

        public DeleteTimeSlotCommandHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(DeleteTimeSlotCommand request,
            CancellationToken cancellationToken)
        {
            var timeSlot = await _dbContext.TimeSlots.FindAsync(request.Id);

            if (timeSlot is null)
            {
                throw new NotFoundException("TimeSlot is not found");
            }

            _dbContext.TimeSlots.Remove(timeSlot);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
