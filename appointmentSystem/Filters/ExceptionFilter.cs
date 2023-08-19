using System.ComponentModel.DataAnnotations;
using System.Net;
using appointmentSystem.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace appointmentSystem.Filters;

public class ExceptionFilter : IExceptionFilter, IOrderedFilter
{
    private readonly ILogger<ExceptionFilter> _logger;

    public ExceptionFilter(ILogger<ExceptionFilter> logger)
    {
        _logger = logger;
    }
    
    public int Order => int.MaxValue - 10;

    public void OnException(ExceptionContext context)
    {
        string errorMessage;
        HttpStatusCode statusCode;

        switch (context.Exception)
        {
            case NotFoundException notFoundException:
                context.Result = new NotFoundResult();
                statusCode = HttpStatusCode.NotFound;
                errorMessage = "Result not found. More details: https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4";
                break;

            case InternalServerErrorException internalServerError:
                statusCode = HttpStatusCode.InternalServerError;
                errorMessage = "An unexpected error occurred. More details: https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1";
                break;

            case BadGatewayException badGateway:
                statusCode = HttpStatusCode.BadGateway;
                errorMessage = "Bad Gateway. More details: https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.3";
                break;
            
            case InvalidOperationException invalidOperationException:
                statusCode = HttpStatusCode.BadRequest;
                errorMessage = "Client with this phone or service with this name have already created. Please change and try again";
                break;

            default:
                statusCode = HttpStatusCode.InternalServerError;
                errorMessage = "An unexpected error occurred. Please try again later.";
                break;
        }

        _logger.LogError(context.Exception, errorMessage);

        var result = new ObjectResult(new
        {
            status = statusCode,
            error = errorMessage
        })
        {
            StatusCode = (int)statusCode
        };

        context.Result = result;
        context.ExceptionHandled = true;
    }
}




