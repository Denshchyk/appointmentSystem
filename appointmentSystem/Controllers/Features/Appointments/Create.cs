using System.ComponentModel.DataAnnotations;
using appointmentSystem.Data;
using appointmentSystem.Models;
using appointmentSystem.Models.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace appointmentSystem.Controllers.Features.Appointments;

[ApiController]
[ApiExplorerSettings(GroupName = "Appointments")]
public class CreateAppointmentController : ControllerBase
{
    private readonly IMediator _mediator;

    public CreateAppointmentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("api/appointment")]
    public async Task<AppointmentViewModel> CreateAppointment(CreateAppointmentCommand command)
    {
        var result = await _mediator.Send(command);

        return result;
    }

    public record CreateAppointmentCommand : IRequest<AppointmentViewModel>
    {
        [Required] public Guid ClientId { get; set; }
        [Required] public Guid TimeSlotId { get; set; }
    };

    public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, AppointmentViewModel>
    {
        private readonly AppDbContext _dbContext;

        public CreateAppointmentCommandHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AppointmentViewModel> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
        {
            var client = await _dbContext.Clients.FirstOrDefaultAsync(c => c.Id == request.ClientId, cancellationToken);
            if(client is null)
            {
                throw new InvalidOperationException($"A Client with id {request.ClientId} is not found.");
            }
            var timeSlot = await _dbContext.TimeSlots.FirstOrDefaultAsync(c => c.Id == request.TimeSlotId, cancellationToken);
            if(timeSlot is null)
            {
                throw new InvalidOperationException($"A TimeSlot with id {request.TimeSlotId} is not found.");
            }

            var appointment = new Appointment
            {
                ClientId = request.ClientId,
                TimeSlotId = request.TimeSlotId,
            };

            await _dbContext.Appointments.AddAsync(appointment, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new AppointmentViewModel(appointment.Id, appointment.ClientId, appointment.TimeSlotId);
        }
    }
}