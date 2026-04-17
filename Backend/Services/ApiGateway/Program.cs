using Yarp.ReverseProxy;

var builder = WebApplication.CreateBuilder(args);

// 1. YARP
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// 2. CORS
var allowedOrigins = builder.Configuration
    .GetSection("AllowedOrigins")
    .Get<string[]>() ?? [];

builder.Services.AddCors(options =>
    options.AddPolicy("FrontendPolicy", policy => policy
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()));  // required for SignalR

// 3. Health Checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// 4. CORS middleware
app.UseCors("FrontendPolicy");

// 5. Health check endpoint
app.MapHealthChecks("/health");

// 6. Debug endpoint for YARP routes
app.MapGet("/gateway/routes", (IProxyStateLookup proxy) =>
{
    var routes = proxy.GetRoutes().Select(r => new
    {
        RouteId  = r.Config.RouteId,
        Path     = r.Config.Match.Path,
        Cluster  = r.Config.ClusterId
    });
    return Results.Ok(routes);
});

// 7. YARP reverse proxy
app.MapReverseProxy();

app.Run();
