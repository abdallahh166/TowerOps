namespace TowerOps.Infrastructure;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.Interfaces.Services;
using TowerOps.Domain.Services;
using TowerOps.Infrastructure.Persistence;
using TowerOps.Infrastructure.Persistence.Repositories;
using TowerOps.Infrastructure.Services;

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
                serviceProvider.GetRequiredService<TowerOps.Infrastructure.Persistence.Interceptors.AuditInterceptor>()
            };

            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
                .AddInterceptors(interceptors);
        });

        // Register interceptors
        services.AddScoped<TowerOps.Infrastructure.Persistence.Interceptors.AuditInterceptor>();

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
        services.AddScoped<ISystemSettingsRepository, SystemSettingsRepository>();
        services.AddScoped<IApplicationRoleRepository, ApplicationRoleRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IDailyPlanRepository, DailyPlanRepository>();
        services.AddScoped<ISyncQueueRepository, SyncQueueRepository>();
        services.AddScoped<ISyncConflictRepository, SyncConflictRepository>();
        services.AddScoped<IAssetRepository, AssetRepository>();
        services.AddScoped<IUnusedAssetRepository, UnusedAssetRepository>();
        services.AddScoped<IPortalReadRepository, PortalReadRepository>();

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
        services.AddScoped<ISettingsEncryptionService, SettingsEncryptionService>();
        services.AddScoped<ISystemSettingsService, SystemSettingsService>();
        services.AddScoped<IUploadedFileValidationService, UploadedFileValidationService>();
        services.AddScoped<IFileMalwareScanService, FileMalwareScanService>();
        services.AddScoped<IOtpService, OtpService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IMfaService, MfaService>();
        services.AddSingleton<IOperationalMetrics, OperationalMetrics>();
        services.AddScoped<SlaEvaluationProcessor>();
        services.AddScoped<UploadScanProcessor>();

        // Domain Services with Infrastructure dependencies (Repository-dependent)
        services.AddScoped<IVisitNumberGeneratorService, VisitNumberGeneratorService>();
        services.AddScoped<IMaterialStockService, MaterialStockService>();
        services.AddScoped<ISlaClockService, SlaClockService>();
        services.AddScoped<IGeoCheckInService, GeoCheckInService>();
        services.AddHostedService<SlaEvaluationHostedService>();
        services.AddHostedService<UploadScanHostedService>();

        // HttpContextAccessor for CurrentUserService
        services.AddHttpContextAccessor();
        services.AddMemoryCache();

        return services;
    }
}
