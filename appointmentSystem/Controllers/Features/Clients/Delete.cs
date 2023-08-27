using System;
using System.Threading;
using System.Threading.Tasks;
using appointmentSystem.Data;
using appointmentSystem.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace appointmentSystem.Controllers.Features.Clients;
[ApiController]
[ApiExplorerSettings(GroupName = "Clients")]
public class DeleteClientController : ControllerBase
{
    private readonly IMediator _mediator;

    public DeleteClientController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpDelete("api/clients/{id}")]
    public async Task<IActionResult> DeleteClient(Guid id)
    {
        await _mediator.Send(new DeleteClientCommand(id));

        return NoContent();
    }
    
    public record DeleteClientCommand(Guid Id) : IRequest;
    
    public class DeleteClientCommandHandler : IRequestHandler<DeleteClientCommand>
    {
        private readonly AppDbContext _dbContext;

        public DeleteClientCommandHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    
        public async Task Handle(DeleteClientCommand request, CancellationToken cancellationToken)
        {
            var client = await _dbContext.Clients.FindAsync(request.Id);

            if (client is null)
            {
                throw new NotFoundException("Client name is not found");
            }
            
            _dbContext.Clients.Remove(client);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}