using System.Net;
using System.Text.Json;
using SportZone.Application.Common.Exceptions;

namespace SportZone.API.Middleware;

// Envuelve TODO el pipeline en un único try/catch: traduce excepciones de negocio a códigos HTTP correctos
// y evita que un error interno exponga el stack trace real al cliente.
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            await WriteErrorAsync(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (ValidationException ex)
        {
            await WriteErrorAsync(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception)
        {
            await WriteErrorAsync(context, HttpStatusCode.InternalServerError, "Ocurrió un error inesperado.");
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new { success = false, message };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
