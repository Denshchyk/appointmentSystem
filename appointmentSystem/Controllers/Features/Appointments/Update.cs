using System.ComponentModel.DataAnnotations;
using appointmentSystem.Data;
using appointmentSystem.Exceptions;
using appointmentSystem.Models.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace appointmentSystem.Controllers.Features.Appointments;

[ApiController]
[ApiExplorerSettings(GroupName = "Appointments")]
public class UpdateAppointmentController : ControllerBase
{
    private readonly IMediator _mediator;

    public UpdateAppointmentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPut("api/appointment")]
    public async Task<IActionResult> UpdateAppointment(Guid id, UpdateAppointmentViewModel updateAppointmentViewModel)
    {
        var updateAppointmentCommand = new UpdateAppointmentCommand(id, updateAppointmentViewModel.ClientId,
            updateAppointmentViewModel.TimeSlotId, updateAppointmentViewModel.ServiceId);
        var result = await _mediator.Send(updateAppointmentCommand);

        return Ok(result);
    }

    public record UpdateAppointmentViewModel
    {
        [Required] public Guid Id { get; set; }
        [Required] public Guid ClientId { get; set; }
        [Required] public Guid TimeSlotId { get; set; }
        [Required] public Guid ServiceId { get; set; }
    };

    public record UpdateAppointmentCommand
        (Guid Id, Guid ClientId, Guid TimeSlotId, Guid ServiceId) : IRequest<AppointmentViewModel>;

    public class UpdateAppointmentCommandHandler : IRequestHandler<UpdateAppointmentCommand, AppointmentViewModel>
    {
        private readonly AppDbContext _dbContext;

        public UpdateAppointmentCommandHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AppointmentViewModel> Handle(UpdateAppointmentCommand request,
            CancellationToken cancellationToken)
        {
            var appointment = await _dbContext.Appointments.FindAsync(request.Id);

            if (appointment is null)
            {
                throw new NotFoundException($"An Appointment with {request.Id} is not found");
            }

            var client = await _dbContext.Clients.FirstOrDefaultAsync(c => c.Id == request.ClientId, cancellationToken);
            if (client is null)
            {
                throw new InvalidOperationException($"A Client with id {request.ClientId} is not found.");
            }

            var timeSlot =
                await _dbContext.TimeSlots.FirstOrDefaultAsync(c => c.Id == request.TimeSlotId, cancellationToken);
            if (timeSlot is null)
            {
                throw new InvalidOperationException($"A TimeSlot with id {request.TimeSlotId} is not found.");
            }

            var service =
                await _dbContext.Services.FirstOrDefaultAsync(c => c.Id == request.ServiceId, cancellationToken);
            if (service is null)
            {
                throw new InvalidOperationException($"A Service with id {request.ServiceId} is not found.");
            }

            appointment.ClientId = request.ClientId;
            appointment.TimeSlotId = request.TimeSlotId;
            appointment.ServiceId = request.ServiceId;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new AppointmentViewModel(appointment.Id, appointment.ClientId, appointment.TimeSlotId,
                appointment.ServiceId);
        }
    }
}
