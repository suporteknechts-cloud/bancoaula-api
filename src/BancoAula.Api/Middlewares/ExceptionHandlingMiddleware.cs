using BancoAula.Api.Exceptions;
using Microsoft.AspNetCore.Mvc;
namespace BancoAula.Api.Middlewares;
public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try { await next(context); }
        catch (Exception ex)
        {
            if (context.Response.HasStarted) throw;
            var status = ex is RegraException regra ? regra.Status : 500;
            if (status == 500) logger.LogError(ex, "Erro inesperado na API");
            context.Response.Clear(); context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(new ProblemDetails { Status = status, Title = status == 500 ? "Erro interno do servidor." : ex.Message });
        }
    }
}
