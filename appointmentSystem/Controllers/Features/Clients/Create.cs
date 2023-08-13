using System.ComponentModel.DataAnnotations;
using appointmentSystem.Data;
using appointmentSystem.Models;
using appointmentSystem.Models.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace appointmentSystem.Controllers.Features.Clients;

[ApiController]
[Route("[controller]")]
[ApiExplorerSettings(GroupName = "Clients")]
public class CreateClientController : ControllerBase
{
    private readonly IMediator _mediator;

    public CreateClientController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("{Name}, {Phone}")]

    public async Task<ClientViewModel> CreateClient(CreateClientCommand command)
        {
            var result = await _mediator.Send(command);

            return result;
        }

    public record CreateClientCommand : IRequest<ClientViewModel>
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        [Required] [StringLength(12)] public string? Phone { get; set; }
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
                var client = new Client
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Phone = request.Phone
                };

                await _dbContext.Clients.AddAsync(client, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return new ClientViewModel(client.Id, client.Name, client.Phone);
            }
    }
}