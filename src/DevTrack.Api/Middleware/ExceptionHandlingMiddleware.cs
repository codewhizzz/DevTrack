using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace DevTrack.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        object response;
        
        if (exception is ValidationException validationException)
        {
            response = new
            {
                status = (int)HttpStatusCode.BadRequest,
                title = "Validation Failed",
                errors = validationException.Errors.Select(e => new
                {
                    field = e.PropertyName,
                    message = e.ErrorMessage
                })
            };
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
        else if (exception is UnauthorizedAccessException)
        {
            response = new
            {
                status = (int)HttpStatusCode.Unauthorized,
                title = "Unauthorized",
                errors = new[] { new { field = "", message = exception.Message } }
            };
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }
        else if (exception is InvalidOperationException)
        {
            response = new
            {
                status = (int)HttpStatusCode.BadRequest,
                title = "Bad Request",
                errors = new[] { new { field = "", message = exception.Message } }
            };
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
        else
        {
            response = new
            {
                status = (int)HttpStatusCode.InternalServerError,
                title = "Internal Server Error",
                errors = new[] { new { field = "", message = "An error occurred while processing your request" } }
            };
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}