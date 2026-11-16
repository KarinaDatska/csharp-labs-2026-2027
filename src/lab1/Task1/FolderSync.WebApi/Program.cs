using System.Text.Json.Serialization;
using FolderSync.Application.Repositories;
using FolderSync.Application.Services;
using FolderSync.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddOpenApi();
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "Folder Sync";
    config.Version = "v1";
    config.Description = "Folder synchronization";
});

// Repositories & Data Providers
builder.Services.AddScoped<ITestFileGenerator, TestFileGenerator>();

// Services
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
app.UseMiddleware<FolderSync.WebApi.Middleware.ErrorMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
}

app.UseCors("AllowAll");

var frontPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot");
if (!Directory.Exists(frontPath))
    frontPath = Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "..", "FolderSync.Front"));
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
