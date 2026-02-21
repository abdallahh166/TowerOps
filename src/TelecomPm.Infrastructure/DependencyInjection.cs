namespace TelecomPM.Infrastructure;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.Interfaces.Services;
using TelecomPM.Domain.Services;
using TelecomPM.Infrastructure.Persistence;
using TelecomPM.Infrastructure.Persistence.Repositories;
using TelecomPM.Infrastructure.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            var interceptors = new[]
            {
                serviceProvider.GetRequiredService<TelecomPM.Infrastructure.Persistence.Interceptors.AuditInterceptor>()
            };

            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
                .AddInterceptors(interceptors);
        });

        // Register interceptors
        services.AddScoped<TelecomPM.Infrastructure.Persistence.Interceptors.AuditInterceptor>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Email Service
        services.AddScoped<IEmailService, EmailService>();

        // Repositories
        services.AddScoped<IVisitRepository, VisitRepository>();
        services.AddScoped<ISiteRepository, SiteRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOfficeRepository, OfficeRepository>();
        services.AddScoped<IMaterialRepository, MaterialRepository>();
        services.AddScoped<IWorkOrderRepository, WorkOrderRepository>();
        services.AddScoped<IEscalationRepository, EscalationRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IApprovalRecordRepository, ApprovalRecordRepository>();
        services.AddScoped<IChecklistTemplateRepository, ChecklistTemplateRepository>();
        services.AddScoped<IBatteryDischargeTestRepository, BatteryDischargeTestRepository>();

        // Domain event dispatcher
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        services.Configure<PushNotificationOptions>(configuration.GetSection("PushNotifications"));
        services.Configure<TwilioOptions>(configuration.GetSection("Twilio"));
        services.AddHttpClient(nameof(NotificationService))
            .AddResilienceHandler("notification-http-resilience", builder =>
            {
                builder.AddRetry(new HttpRetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    BackoffType = DelayBackoffType.Exponential
                });

                builder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
                {
                    MinimumThroughput = 5,
                    FailureRatio = 1.0,
                    BreakDuration = TimeSpan.FromSeconds(30)
                });

                builder.AddTimeout(TimeSpan.FromSeconds(10));
            });

        // Infrastructure Services (External concerns & I/O)
        services.AddScoped<IDateTimeService, DateTimeService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IFileStorageService, BlobStorageService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IExcelExportService, ExcelExportService>();
        services.AddScoped<IReportGenerationService, ReportGenerationService>();

        // Domain Services with Infrastructure dependencies (Repository-dependent)
        services.AddScoped<IVisitNumberGeneratorService, VisitNumberGeneratorService>();
        services.AddScoped<IMaterialStockService, MaterialStockService>();
        services.AddScoped<ISlaClockService, SlaClockService>();

        // HttpContextAccessor for CurrentUserService
        services.AddHttpContextAccessor();

        return services;
    }
}
