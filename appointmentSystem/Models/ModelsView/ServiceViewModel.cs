namespace appointmentSystem.Models.Models;

public record ServiceViewModel(Guid Id, string Name, string Description, int DurationInMinutes, decimal Price);