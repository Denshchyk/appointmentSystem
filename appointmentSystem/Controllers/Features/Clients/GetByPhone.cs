using appointmentSystem.Data;
using appointmentSystem.Exceptions;
using appointmentSystem.Models.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace appointmentSystem.Controllers.Features.Clients;

[ApiController]
[ApiExplorerSettings(GroupName = "Clients")]
public class GetClientByPhoneController : ControllerBase
{
    private readonly IMediator _mediator;

    public GetClientByPhoneController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("api/clients/{phone}")]
    public async Task<IActionResult> GetClient(string phone)
    {
        var clientModel = await _mediator.Send(new GetClientByPhoneQuery(phone));

        return Ok(clientModel);
    }
    
    public record GetClientByPhoneQuery(string phone) : IRequest<ClientViewModel>;
    
    public class GetClientByPhoneQueryHandler : IRequestHandler<GetClientByPhoneQuery, ClientViewModel>
    {
        private readonly AppDbContext _dbContext;

        public GetClientByPhoneQueryHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    
        public async Task<ClientViewModel> Handle(GetClientByPhoneQuery request, CancellationToken cancellationToken)
        {
            var client = await _dbContext.Clients.FindAsync(request.phone);

            if (client == null)
            {
                throw new NotFoundException("Client is not found");
            }
            
            return new ClientViewModel(client.Id, client.Name, client.Phone);
        }
    }
}