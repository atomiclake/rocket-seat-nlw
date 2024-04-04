using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using PassIn.Communication.Responses;
using PassIn.Exceptions;

using System.Net;

namespace PassIn.Api.Filters;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is PassInException)
        {
            HandleProjectException(context);
        }
        else
        {
            ThrowUnknownError(context);
        }
    }

    private void HandleProjectException(ExceptionContext context)
    {
        var errorResponse = new ResponseErrorJson(context.Exception.Message);

        if (context.Exception is NotFoundException)
        {
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
            context.Result = new NotFoundObjectResult(errorResponse);
        }
        else if (context.Exception is ErrorOnValidationException)
        {
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Result = new BadRequestObjectResult(errorResponse);
        }
        else if (context.Exception is RequestConflictException)
        {
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
            context.Result = new ConflictObjectResult(errorResponse);
        }
    }

    private void ThrowUnknownError(ExceptionContext context)
    {
        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Result = new ObjectResult(new ResponseErrorJson("Unknown error."));
    }
}
