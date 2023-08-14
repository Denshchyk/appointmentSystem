using appointmentSystem.Data;
using appointmentSystem.Exceptions;
using appointmentSystem.Models;
using appointmentSystem.Models.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace appointmentSystem.Controllers.Features.Clients;


[ApiController]
[ApiExplorerSettings(GroupName = "Clients")]
public class ClientsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClientsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("api/clients")]
    public async Task<IActionResult> GetAllClients()
    {
        var clients = await _mediator.Send(new GetAllClientsQuery());

        return Ok(clients);
    }
    
    public record GetAllClientsQuery : IRequest<List<ClientViewModel>>;
    
    public class GetAllClientsQueryHandler : IRequestHandler<GetAllClientsQuery, List<ClientViewModel>>
    {
        private readonly AppDbContext _dbContext;

        public GetAllClientsQueryHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    
        public async Task<List<ClientViewModel>> Handle(GetAllClientsQuery request, CancellationToken cancellationToken)
        {
            var clients = await _dbContext.Clients.ToListAsync(cancellationToken);

            return clients.Select(client => new ClientViewModel(client.Id, client.Name, client.Phone)).ToList();
        }
    }
}
