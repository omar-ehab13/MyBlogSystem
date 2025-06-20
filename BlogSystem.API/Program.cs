using BlogSystem.API.Extensions;
using BlogSystem.API.Middleware;
using BlogSystem.Application;
using BlogSystem.Infrastructure;
using BlogSystem.Infrastructure.Data;
using BlogSystem.Infrastructure.Logging;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, loggerConfig) => 
    loggerConfig.ReadFrom.Configuration(context.Configuration));

// Application Layer Services
builder.Services.ConfigureMappingService();
builder.Services.ConfigureMediatR();
builder.Services.ConfigureFluentValidation();

// Infrastructure Layer Services
builder.Services.ConfigureDbContext(builder.Configuration);
builder.Services.RegisterRepositories();

// Add Presentation (API) Layer Configurations
builder.Services.ConfigureCORS();
builder.Services.ConfigureSwagger();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (true)
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BlogSystem API v1");
        c.RoutePrefix = string.Empty;
        c.DocumentTitle = "BlogSystem API Documentation";
        c.DefaultModelsExpandDepth(-1); // Hide schemas section by default
        c.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
        c.DisplayRequestDuration();
        c.EnableDeepLinking();
        c.EnableFilter();
        c.ShowExtensions();
        c.EnableValidator();
    });
}

app.UseGlobalExceptionHandler();

app.UseHttpsRedirection();

app.UseMiddleware<RequestLogContextMiddleware>();

app.UseSerilogRequestLogging();

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

// Auto migrate
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BlogSystemDbContext>();
    await context.Database.MigrateAsync();
}

app.Run();
