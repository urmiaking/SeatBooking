using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace SeatBooking.API.Controllers;

[ApiController]
public abstract class ApiControllerBase : Controller
{
    protected IActionResult Problem(List<Error> errors)
    {
        if (errors.Count is 0)
        {
            return Problem();
        }

        if (errors.All(error => error.Type == ErrorType.Validation))
        {
            return ValidationProblem(errors);
        }

        HttpContext.Items["Errors"] = errors;

        return Problem(errors[0]);
    }

    private IActionResult ValidationProblem(List<Error> errors)
    {
        var modelStateDictionary = new ModelStateDictionary();

        foreach (var error in errors)
        {
            modelStateDictionary.AddModelError(error.Code, error.Description);
        }

        return ValidationProblem(modelStateDictionary);
    }

    private IActionResult Problem(Error error)
    {
        var statusCode = GetStatusCode(error.Type);
        return Problem(statusCode: statusCode, title: error.Description);
    }

    private static int GetStatusCode(ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError,
        };
    }
}