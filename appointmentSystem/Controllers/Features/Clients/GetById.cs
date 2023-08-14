using appointmentSystem.Data;
using appointmentSystem.Exceptions;
using appointmentSystem.Models.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace appointmentSystem.Controllers.Features.Clients;

[ApiController]
[ApiExplorerSettings(GroupName = "Clients")]
public class GetClientByIdController : ControllerBase
{
    private readonly IMediator _mediator;

    public GetClientByIdController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("api/clients/{id}")]
    public async Task<IActionResult> GetClient(Guid id)
    {
        var clientModel = await _mediator.Send(new GetClientByIdQuery(id));

        return Ok(clientModel);
    }
    
    public record GetClientByIdQuery(Guid Id) : IRequest<ClientViewModel>;
    
    public class GetClientByIdQueryHandler : IRequestHandler<GetClientByIdQuery, ClientViewModel>
    {
        private readonly AppDbContext _dbContext;

        public GetClientByIdQueryHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    
        public async Task<ClientViewModel> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
        {
            var client = await _dbContext.Clients.FindAsync(request.Id);

            if (client == null)
            {
                throw new NotFoundException("Client name is not found");
            }
            
            return new ClientViewModel(client.Id, client.Name, client.Phone);
        }
    }
}