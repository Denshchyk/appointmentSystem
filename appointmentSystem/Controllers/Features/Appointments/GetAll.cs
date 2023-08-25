using appointmentSystem.Data;
using appointmentSystem.Exceptions;
using appointmentSystem.Models.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace appointmentSystem.Controllers.Features.Appointments;

[ApiController]
[ApiExplorerSettings(GroupName = "Appointments")]
public class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AppointmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("api/appointment")]
    public async Task<IActionResult> GetAllAppointments()
    {
        var appointments = await _mediator.Send(new GetAllAppointmentsQuery());

        return Ok(appointments);
    }
    
    public record GetAllAppointmentsQuery : IRequest<List<AppointmentViewModel>>;
    
    public class GetAllAppointmentsQueryHandler : IRequestHandler<GetAllAppointmentsQuery, List<AppointmentViewModel>>
    {
        private readonly AppDbContext _dbContext;

        public GetAllAppointmentsQueryHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    
        public async Task<List<AppointmentViewModel>> Handle(GetAllAppointmentsQuery request, CancellationToken cancellationToken)
        {
            var appointments = await _dbContext.Appointments.ToListAsync(cancellationToken);
            
            if (appointments is null)
            {
                throw new NotFoundException("Appointments are not found");
            }

            return appointments.Select(appointment => new AppointmentViewModel(appointment.Id, appointment.ClientId, appointment.TimeSlotId, appointment.ServiceId)).ToList();
        }
    }
}