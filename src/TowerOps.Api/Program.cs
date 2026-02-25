using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using TowerOps.Api.Filters;
using TowerOps.Api.Localization;
using TowerOps.Api.Middleware;
using TowerOps.Api.Authorization;
using TowerOps.Api.Security;
using TowerOps.Api.Services;
using TowerOps.Application;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Infrastructure;
using TowerOps.Infrastructure.Persistence;
using TowerOps.Infrastructure.Persistence.SeedData;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/towerops-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<ILocalizedTextService, LocalizedTextService>();
builder.Services.AddScoped<IValidationErrorLocalizer, ValidationErrorLocalizer>();
builder.Services.AddLocalization();

var supportedCultureCodes = builder.Configuration
    .GetSection("Localization:SupportedCultures")
    .Get<string[]>();

if (supportedCultureCodes is null || supportedCultureCodes.Length == 0)
{
    supportedCultureCodes = new[] { "en-US", "ar-EG" };
}

var supportedCultures = supportedCultureCodes
    .Select(code => new CultureInfo(code))
    .ToList();

var defaultCultureCode = builder.Configuration["Localization:DefaultCulture"];
if (string.IsNullOrWhiteSpace(defaultCultureCode) ||
    supportedCultures.All(c => !string.Equals(c.Name, defaultCultureCode, StringComparison.OrdinalIgnoreCase)))
{
    defaultCultureCode = supportedCultures[0].Name;
}

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(defaultCultureCode);
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.RequestCultureProviders = new IRequestCultureProvider[]
    {
        new QueryStringRequestCultureProvider(),
        new AcceptLanguageHeaderRequestCultureProvider(),
        new CookieRequestCultureProvider()
    };
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidateModelStateFilter>();
})
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

var apiSecurityOptions = ApiSecurityHardeningOptions.FromConfiguration(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        var allowedOrigins = ApiSecurityHardening.ResolveAllowedOrigins(
            builder.Configuration,
            builder.Environment.IsProduction());

        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddHsts(options =>
{
    var maxAgeDays = builder.Configuration.GetValue<int?>("Hsts:MaxAgeDays") ?? 180;
    options.MaxAge = TimeSpan.FromDays(maxAgeDays > 0 ? maxAgeDays : 180);
    options.IncludeSubDomains = builder.Configuration.GetValue<bool?>("Hsts:IncludeSubDomains") ?? true;
    options.Preload = builder.Configuration.GetValue<bool?>("Hsts:Preload") ?? true;
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = ApiSecurityHardening.CreateGlobalLimiter(apiSecurityOptions);
});

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET");

if (string.IsNullOrWhiteSpace(secretKey) && builder.Environment.IsProduction())
{
    throw new InvalidOperationException("JWT_SECRET environment variable is required in Production.");
}

if (string.IsNullOrWhiteSpace(secretKey))
{
    secretKey = jwtSettings["Secret"];
}

if (string.IsNullOrWhiteSpace(secretKey))
{
    throw new InvalidOperationException("JWT secret key is not configured.");
}

builder.Configuration["JwtSettings:Secret"] = secretKey;

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

builder.Services.AddAuthorization(ApiAuthorizationPolicies.Configure);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TowerOps API",
        Version = "v1",
        Description = "TowerOps - Telecom Field Operations Platform for CM/PM subcontractors",
        Contact = new OpenApiContact
        {
            Name = "Seven Pictures - TowerOps Team",
            Email = "support@sevenpictures.com"
        }
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

var app = builder.Build();
var localizationOptions = app.Services
    .GetRequiredService<Microsoft.Extensions.Options.IOptions<RequestLocalizationOptions>>()
    .Value;
app.UseRequestLocalization(localizationOptions);

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TowerOps API V1");
        c.RoutePrefix = string.Empty;
    });
}

if (app.Environment.IsProduction())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

if (app.Environment.IsDevelopment())
{
    try
    {
        await app.SeedDatabaseAsync();
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning(ex,
            "Startup continued without database seeding. Set a supported SQL Server connection string in DefaultConnection.");
    }
}

app.Run();
