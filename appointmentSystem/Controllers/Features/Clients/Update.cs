using System.ComponentModel.DataAnnotations;
using appointmentSystem.Data;
using appointmentSystem.Exceptions;
using appointmentSystem.Models;
using appointmentSystem.Models.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace appointmentSystem.Controllers.Features.Clients;

[ApiController]
[ApiExplorerSettings(GroupName = "Clients")]
public class UpdateClientController : ControllerBase
{
    private readonly IMediator _mediator;

    public UpdateClientController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPut("api/clients/{id}")]
    public async Task<IActionResult> UpdateClient(Guid id, UpdateClientViewModel updateClientViewModel)
    {
        var updateClientCommand = new UpdateClientCommand(id, updateClientViewModel.Name, updateClientViewModel.Phone);
        var result = await _mediator.Send(updateClientCommand);

        return Ok(result);
    }

    public record UpdateClientViewModel
    {
        [Required] [StringLength(20)] public string Name { get; set; }
        [Required] [StringLength(12)] public string Phone { get; set; }
    };

    public record UpdateClientCommand(Guid Id, string Name, string Phone) : IRequest<ClientViewModel>;

    public class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand, ClientViewModel>
    {
        private readonly AppDbContext _dbContext;

        public UpdateClientCommandHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ClientViewModel> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
        {
            var client = await _dbContext.Clients.FindAsync(request.Id);

            if (client is null)
            {
                throw new NotFoundException("Client is not found");
            }

            client.Name = request.Name;
            client.Phone = request.Phone;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new ClientViewModel(client.Id, client.Name, client.Phone);
        }
    }
}