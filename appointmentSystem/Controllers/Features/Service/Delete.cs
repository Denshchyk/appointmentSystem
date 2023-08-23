using System;
using System.Threading;
using System.Threading.Tasks;
using appointmentSystem.Data;
using appointmentSystem.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace appointmentSystem.Controllers.Features.Service;

[ApiController]
[ApiExplorerSettings(GroupName = "Services")]
public class DeleteServiceController : ControllerBase
{
    private readonly IMediator _mediator;

    public DeleteServiceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpDelete("api/services/{id}")]
    public async Task<IActionResult> DeleteService(Guid id)
    {
        await _mediator.Send(new DeleteServiceCommand(id));

        return NoContent();
    }

    public record DeleteServiceCommand(Guid Id) : IRequest;

    public class DeleteServiceCommandHandler : IRequestHandler<DeleteServiceCommand>
    {
        private readonly AppDbContext _dbContext;

        public DeleteServiceCommandHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(DeleteServiceCommand request,
            CancellationToken cancellationToken)
        {
            var service = await _dbContext.Services.FindAsync(request.Id);

            if (service is null)
            {
                throw new NotFoundException("Service name is not found");
            }

            _dbContext.Services.Remove(service);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
