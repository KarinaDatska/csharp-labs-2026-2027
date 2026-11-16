namespace lab1.WebApi.Middleware;

public class ErrorMiddleware(RequestDelegate next, ILogger<ErrorMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
            if (context.Request.Method != "GET" || context.Request.Path.Value?.Contains("export") == true
                || context.Request.Path.Value?.Contains("Reports", StringComparison.OrdinalIgnoreCase) == true)
                context.RequestServices.GetRequiredService<lab1.Infrastructure.Logging.EventLog>().Write($"{context.Request.Method} {context.Request.Path} {context.Response.StatusCode}");
        }
        catch (Exception error)
        {
            logger.LogError(error, "Request failed");
            context.RequestServices.GetRequiredService<lab1.Infrastructure.Logging.EventLog>().Write($"ERROR {context.Request.Path} {error.Message}");
            context.Response.StatusCode = error is KeyNotFoundException ? 404 :
                error is ArgumentException or FormatException or IOException or UnauthorizedAccessException
                or InvalidOperationException or System.Text.Json.JsonException or System.Xml.XmlException
                or CsvHelper.CsvHelperException or DocumentFormat.OpenXml.Packaging.OpenXmlPackageException ? 400 : 500;
            await context.Response.WriteAsJsonAsync(new { message = context.Response.StatusCode == 500
                ? "Не вдалося виконати операцію. Перевірте файл та налаштування імпорту."
                : "Перевірте формат, кодування, роздільник та доступ до файлу. " + error.Message });
        }
    }
}
