namespace appointmentSystem.Models.Models;

public record AppointmentViewModel(Guid Id, Guid ClientId, Guid TimeSlotId, Guid ServiceId);