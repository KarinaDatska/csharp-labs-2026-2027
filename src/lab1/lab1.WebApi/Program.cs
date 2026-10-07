using System.Text.Json.Serialization;
using lab1.Application.Repositories;
using lab1.Application.Services;
using lab1.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "Lab1 - Air Quality";
    config.Version = "v1";
    config.Description = "FileSystem and data work (Air Quality analysis, import/export, charts and reports)";
});

// Repositories & Data Providers
builder.Services.AddSingleton<IAirQualityDataStore, AirQualityDataStore>();
builder.Services.AddScoped<ICsvAirQualityProvider, CsvAirQualityProvider>();
builder.Services.AddScoped<IJsonAirQualityProvider, JsonAirQualityProvider>();
builder.Services.AddScoped<IXmlAirQualityProvider, XmlAirQualityProvider>();
builder.Services.AddScoped<IXlsxAirQualityProvider, XlsxDataProvider>();
builder.Services.AddScoped<IScottPlotChartService, ScottPlotChartService>();
builder.Services.AddScoped<IXlsxReportProvider, XlsxReportProvider>();
builder.Services.AddScoped<IDocxReportProvider, DocxReportProvider>();
builder.Services.AddScoped<ITestFileGenerator, TestFileGenerator>();

// Services
builder.Services.AddScoped<ICsvAirQualityService, CsvAirQualityService>();
builder.Services.AddScoped<IJsonAirQualityService, JsonAirQualityService>();
builder.Services.AddScoped<IXmlAirQualityService, XmlAirQualityService>();
builder.Services.AddScoped<IXlsxAirQualityService, XlsxAirQualityService>();
builder.Services.AddScoped<IDataAnalyticsService, DataAnalyticsService>();
builder.Services.AddScoped<IChartDataService, ChartDataService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IFolderSyncService, FolderSyncService>();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
}

app.UseCors("AllowAll");

var frontPath = Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "..", "lab1.Front"));
if (Directory.Exists(frontPath))
{
    app.UseFileServer(new FileServerOptions
    {
        FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(frontPath),
        RequestPath = "",
        EnableDefaultFiles = true
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
