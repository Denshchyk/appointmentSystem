using appointmentSystem.Data;
using appointmentSystem.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace appointmentSystem.Controllers.Features.Appointments;

[ApiController]
[ApiExplorerSettings(GroupName = "Appointments")]
public class DeleteAppointmentController : ControllerBase
{
    private readonly IMediator _mediator;

    public DeleteAppointmentController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpDelete("api/appointment/{id}")]
    public async Task<IActionResult> DeleteAppointment(Guid id)
    {
        await _mediator.Send(new DeleteAppointmentCommand(id));

        return NoContent();
    }
    
    public record DeleteAppointmentCommand(Guid Id) : IRequest;
    
    public class DeleteAppointmentCommandHandler : IRequestHandler<DeleteAppointmentCommand>
    {
        private readonly AppDbContext _dbContext;

        public DeleteAppointmentCommandHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    
        public async Task Handle(DeleteAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _dbContext.Appointments.FindAsync(request.Id);

            if (appointment is null)
            {
                throw new NotFoundException("Appointment is not found");
            }
            
            _dbContext.Appointments.Remove(appointment);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}