using appointmentSystem.Data;
using appointmentSystem.Exceptions;
using appointmentSystem.Models.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace appointmentSystem.Controllers.Features.Appointments;

[ApiController]
[ApiExplorerSettings(GroupName = "Appointments")]
public class GetAppointmentByIdController : ControllerBase
{
    private readonly IMediator _mediator;

    public GetAppointmentByIdController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("api/appointment/{id}")]
    public async Task<IActionResult> GetAppointment(Guid id)
    {
        var appointmentModel = await _mediator.Send(new GetAppointmentByIdQuery(id));

        return Ok(appointmentModel);
    }
    
    public record GetAppointmentByIdQuery(Guid Id) : IRequest<AppointmentViewModel>;
    
    public class GetAppointmentByIdQueryHandler : IRequestHandler<GetAppointmentByIdQuery, AppointmentViewModel>
    {
        private readonly AppDbContext _dbContext;

        public GetAppointmentByIdQueryHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    
        public async Task<AppointmentViewModel> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
        {
            var appointment = await _dbContext.Appointments.FindAsync(request.Id);

            if (appointment == null)
            {
                throw new NotFoundException("Appointment is not found");
            }

            return new AppointmentViewModel(appointment.Id, appointment.ClientId, appointment.TimeSlotId,
                appointment.ServiceId);
        }
    }
}