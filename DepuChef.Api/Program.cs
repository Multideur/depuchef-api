using DepuChef.Api;
using DepuChef.Api.Policies;
using DepuChef.Application.Models;
using DepuChef.Application.Models.OpenAI;
using DepuChef.Application.Services;
using DepuChef.Application.Services.OpenAi;
using DepuChef.Application.Utilities;
using DepuChef.Infrastructure;
using DepuChef.Infrastructure.Hubs;
using DepuChef.Infrastructure.Services;
using DepuChef.Infrastructure.Services.OpenAi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
var configuration = builder.Configuration;

services.AddScoped<ICleanUpService, CleanUpService>();
services.AddScoped<IFileManager, FileManager>();
services.AddScoped<IMessageManager, MessageManager>();
services.AddScoped<IThreadManager, ThreadManager>();
services.AddScoped<IRecipeService, OpenAiRecipeService>();
services.AddScoped<IUserService, UserService>();
services.AddScoped<IHttpService, HttpService>();
services.AddScoped<IClientNotifier, ClientNotifier>();
services.AddScoped<IJsonFileReader, JsonFileReader>();
services.AddSingleton(Channel.CreateUnbounded<BackgroundRecipeRequest>());
services.AddSingleton<IRecipeRequestBackgroundService>(provider =>
{
    var logger = provider.GetRequiredService<ILogger<RecipeRequestBackgroundService>>();
    var channel = provider.GetRequiredService<Channel<BackgroundRecipeRequest>>();

    return new RecipeRequestBackgroundService(provider, channel, logger);
});
services.AddHostedService(provider =>
    (RecipeRequestBackgroundService)provider.GetRequiredService<IRecipeRequestBackgroundService>());

services.Configure<OpenAiOptions>(configuration.GetSection(OpenAiOptions.Options));
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
services.AddSignalR();

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

services.ConfigureDatabase(configuration.GetSection("ConnectionStrings"));

var swaggerInfo = new OpenApiInfo
{
    Version = "v1",
    Title = "Depuchef API",
    Description = "Depuchef backend for mobile app"
};

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

app.AddEndpoints();
app.MapHub<NotificationHub>("/hub");

app.Run();
