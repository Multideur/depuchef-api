using Azure.Monitor.OpenTelemetry.AspNetCore;
using DepuChef.Api.Extensions;
using DepuChef.Api.Middleware;
using DepuChef.Api.Policies;
using DepuChef.Api.Validators;
using DepuChef.Application;
using DepuChef.Application.Models;
using DepuChef.Application.Models.OpenAI;
using DepuChef.Application.Services;
using DepuChef.Application.Services.OpenAi;
using DepuChef.Application.Utilities;
using DepuChef.Infrastructure;
using DepuChef.Infrastructure.Hubs;
using DepuChef.Infrastructure.Services;
using DepuChef.Infrastructure.Services.OpenAi;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseSentry(options =>
{
    options.Dsn = builder.Configuration.GetSection("Sentry:Dsn").Value;
#if DEBUG
    options.Debug = true;
#endif
});

builder.Logging.AddSentry();

var services = builder.Services;

// Register Validators
services.AddValidatorsFromAssemblyContaining<RegisterUserRequestValidator>();

services.AddScoped<ICleanUpService, CleanUpService>();
services.AddScoped<IFileManager, FileManager>();
services.AddScoped<IMessageManager, MessageManager>();
services.AddScoped<IThreadManager, ThreadManager>();
services.AddScoped<IAiRecipeService, OpenAiRecipeService>();
services.AddScoped<IRecipeService, RecipeService>();
services.AddScoped<IUserService, UserService>();
services.AddScoped<IHttpService, HttpService>();
services.AddScoped<IClientNotifier, ClientNotifier>();
services.AddScoped<IJsonFileReader, JsonFileReader>();
services.AddScoped<IClaimsHelper, ClaimsHelper>();
services.AddScoped<IStorageService, AzureStorageService>();
services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
services.AddSingleton(Channel.CreateUnbounded<BackgroundRecipeRequest>());
services.AddSingleton<IRecipeRequestBackgroundService>(provider =>
{
    var logger = provider.GetRequiredService<ILogger<RecipeRequestBackgroundService>>();
    var channel = provider.GetRequiredService<Channel<BackgroundRecipeRequest>>();

    return new RecipeRequestBackgroundService(provider, channel, logger);
});
services.AddHostedService(provider =>
    (RecipeRequestBackgroundService)provider.GetRequiredService<IRecipeRequestBackgroundService>());

var configuration = builder.Configuration;
services.Configure<OpenAiOptions>(configuration.GetSection(OpenAiOptions.Options));
services.Configure<StorageOptions>(configuration.GetSection(StorageOptions.Options));

services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.SetIsOriginAllowed(origin => true)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});
services.AddSignalR()
    .AddAzureSignalR();
services.AddProblemDetails();

var authority = configuration.GetSection("Authentication:Authority").Value;
services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Authority = authority;
        options.Audience = configuration.GetSection("Authentication:Audience").Value;
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("Authentication failed.");
                return Task.CompletedTask;
            }
        };
    });

services.AddAuthorization(options =>
{
    typeof(AuthPolicies)
        .GetAllPublicConstantValues<string>()
        .ToList()
        .ForEach(policyName =>
        {
            options.AddPolicy(policyName!, policy =>
            {
                policy.Requirements.Add(new HasScopeRequirement(policyName!, $"{authority}/"));
            });
        });
});

var connectionString = configuration.GetConnectionString("DepuChef");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string is missing.");
}
services.ConfigureDatabase(connectionString);

var swaggerInfo = new OpenApiInfo
{
    Version = "v1",
    Title = "Depuchef API",
    Description = "Depuchef backend for mobile app",
};

services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(swaggerInfo.Version, swaggerInfo);

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme.
            Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            []
        }
    });
});

services.AddOpenTelemetry().UseAzureMonitor(options =>
{
    options.SamplingRatio = 0.0F; // no sampling
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();

app.UseAuthentication().UseAuthorization();

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.AddEndpoints();
app.MapHub<NotificationHub>("/hub");

app.Run();
