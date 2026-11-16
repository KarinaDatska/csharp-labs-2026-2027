using System.Text.Json.Serialization;
using lab1.Application.Repositories;
using lab1.Application.Services;
using lab1.Infrastructure.Repositories;

System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddOpenApi();
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "Lab1 - Air Quality";
    config.Version = "v2";
    config.Description = "Air Quality analysis, import/export, charts and reports";
});

// Repositories i Data Providers
builder.Services.AddSingleton<IAirQualityDataStore, AirQualityDataStore>();
builder.Services.AddScoped<ICsvAirQualityProvider, CsvAirQualityProvider>();
builder.Services.AddScoped<IJsonAirQualityProvider, JsonAirQualityProvider>();
builder.Services.AddScoped<IXmlAirQualityProvider, XmlAirQualityProvider>();
builder.Services.AddScoped<IXlsxAirQualityProvider, XlsxDataProvider>();
builder.Services.AddScoped<IScottPlotChartService, ScottPlotChartService>();
builder.Services.AddScoped<IXlsxReportProvider, XlsxReportProvider>();
builder.Services.AddScoped<IDocxReportProvider, DocxReportProvider>();

builder.Services.AddScoped<lab1.Application.DTOs.FileOptions>();
builder.Services.AddScoped<RecordService>();
builder.Services.AddSingleton<lab1.Infrastructure.Logging.EventLog>();
builder.Services.AddSingleton<RecentFileStore>();

// Services
builder.Services.AddScoped<ICsvAirQualityService, CsvAirQualityService>();
builder.Services.AddScoped<IJsonAirQualityService, JsonAirQualityService>();
builder.Services.AddScoped<IXmlAirQualityService, XmlAirQualityService>();
builder.Services.AddScoped<IXlsxAirQualityService, XlsxAirQualityService>();
builder.Services.AddScoped<IDataAnalyticsService, DataAnalyticsService>();
builder.Services.AddScoped<IChartDataService, ChartDataService>();
builder.Services.AddScoped<IReportService, ReportService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

var app = builder.Build();
app.UseMiddleware<lab1.WebApi.Middleware.ErrorMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v2.json";
    });
}

app.UseCors("AllowAll");

var frontPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot");
if (!Directory.Exists(frontPath))
    frontPath = Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "..", "lab1.Front"));
if (Directory.Exists(frontPath))
{
    app.UseFileServer(new FileServerOptions
    {
        FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(frontPath),
        RequestPath = "",
        EnableDefaultFiles = true
    });
}



app.UseAuthorization();

app.MapGet("/api/health", () => Results.Ok(new { status = "ok" }));
app.MapControllers();

app.Run();
