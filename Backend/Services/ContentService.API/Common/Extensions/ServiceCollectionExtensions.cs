using ContentService.API.Common.Behaviors;
using ContentService.API.Data;
using ContentService.API.Infrastructure.Http;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace ContentService.API.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddContentService(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ContentDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres")));

        // Health Checks
        services.AddHealthChecks()
            .AddDbContextCheck<ContentDbContext>();

        // MediatR
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // FileService HTTP Client
        services.Configure<FileServiceOptions>(configuration.GetSection("FileService"));

        services.AddHttpClient<IFileServiceClient, FileServiceClient>((serviceProvider, client) =>
        {
            var options = serviceProvider
                .GetRequiredService<IOptions<FileServiceOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });

        return services;
    }
}
