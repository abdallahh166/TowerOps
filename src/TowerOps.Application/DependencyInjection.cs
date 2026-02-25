namespace TowerOps.Application;

using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TowerOps.Application.Common.Behaviors;
using TowerOps.Application.Common.Validation;
using TowerOps.Application.Services;
using TowerOps.Domain.Entities.Users;
using TowerOps.Domain.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
        });

        // AutoMapper
        services.AddAutoMapper(assembly);

        // FluentValidation
        if (ValidatorOptions.Global.LanguageManager is not TowerOpsLanguageManager)
            ValidatorOptions.Global.LanguageManager = new TowerOpsLanguageManager();

        services.AddValidatorsFromAssembly(assembly);

        // Pipeline Behaviors (Order matters!)
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(OperationalMetricsBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        // Application Services (Pure domain logic, no external dependencies)
        services.AddScoped<IVisitValidationService, VisitValidationService>();
        services.AddScoped<ISiteAssignmentService, SiteAssignmentService>();
        services.AddScoped<IVisitDurationCalculatorService, VisitDurationCalculatorService>();
        services.AddScoped<IPhotoChecklistGeneratorService, PhotoChecklistGeneratorService>();
        services.AddScoped<IEditableVisitMutationService, EditableVisitMutationService>();
        services.AddScoped<IVisitApprovalPolicyService, VisitApprovalPolicyService>();
        services.AddScoped<IEscalationRoutingService, EscalationRoutingService>();
        services.AddScoped<IEvidencePolicyService, EvidencePolicyService>();
        services.AddScoped<ISyncQueueProcessor, SyncQueueProcessor>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        return services;
    }
}
