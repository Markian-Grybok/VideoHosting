using Microsoft.EntityFrameworkCore;
using FileService.Infrastructure.Persistence;
using FileService.Infrastructure.Storage;
using FileService.Infrastructure.Messaging;
using MediatR;
using System.Reflection;
using FluentValidation;
using FileService.Common.Behaviors;
using FileService.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using FileService.Features.Processing.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Service registrations
builder.Services.AddControllers();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddDbContext<FileServiceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<MinioOptions>(builder.Configuration.GetSection("Minio"));
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddSingleton<IStorageService, MinioStorageService>();
builder.Services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();
builder.Services.AddHostedService<RabbitMqConsumer>();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddCors(options =>
    options.AddPolicy("SignalRPolicy", policy => policy
        .WithOrigins("http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()));

var app = builder.Build();

// Exception handler
app.UseExceptionHandler(errApp => errApp.Run(async context =>
{
    var feature = context.Features.Get<IExceptionHandlerFeature>();
    var ex = feature?.Error;
    var (status, title) = ex switch
    {
        ValidationException ve => (400, ve.Message),
        StorageException      => (502, "Storage error"),
        ProcessingException   => (500, "Processing error"),
        KeyNotFoundException ke => (404, ke.Message),
        InvalidOperationException ioe => (400, ioe.Message),
        _                     => (500, "Internal server error")
    };
    context.Response.StatusCode = status;
    context.Response.ContentType = "application/problem+json";
    await context.Response.WriteAsJsonAsync(new { title, status });
}));

// Swagger (dev only)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS
app.UseCors("SignalRPolicy");

// Authentication/Authorization
app.UseAuthentication();
app.UseAuthorization();

// Controllers
app.MapControllers();

// SignalR hub
app.MapHub<ProcessingHub>("/hubs/processing");

app.Run();
