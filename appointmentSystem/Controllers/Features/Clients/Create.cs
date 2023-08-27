using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using appointmentSystem.Data;
using appointmentSystem.Models;
using appointmentSystem.Models.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace appointmentSystem.Controllers.Features.Clients;

[ApiController]
[ApiExplorerSettings(GroupName = "Clients")]
public class CreateClientController : ControllerBase
{
    private readonly IMediator _mediator;

    public CreateClientController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("api/clients")]
    public async Task<ClientViewModel> CreateClient(CreateClientCommand command)
    {
        var result = await _mediator.Send(command);

        return result;
    }

    public record CreateClientCommand : IRequest<ClientViewModel>
    {
        [Required] [StringLength(20)] public string Name { get; set; }
        [Required] [StringLength(12)] public string Phone { get; set; }
    };

    public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, ClientViewModel>
    {
        private readonly AppDbContext _dbContext;

        public CreateClientCommandHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ClientViewModel> Handle(CreateClientCommand request, CancellationToken cancellationToken)
        {
            var existingClient = await _dbContext.Clients.FirstOrDefaultAsync(c => c.Phone == request.Phone, cancellationToken);
            if(existingClient != null)
            {
                throw new InvalidOperationException($"A client with the phone {request.Phone} already exists.");
            }
            var client = new Client
            {
                Name = request.Name,
                Phone = request.Phone
            };

            await _dbContext.Clients.AddAsync(client, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new ClientViewModel(client.Id, client.Name, client.Phone);
        }
    }
}