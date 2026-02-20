using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TelecomPm.Api.Controllers;
using TelecomPM.Api.Authorization;
using Xunit;

namespace TelecomPM.Application.Tests.Services;

public class ApiAuthorizationPoliciesTests
{
    [Fact]
    public void Configure_ShouldRegisterAllRequiredPolicies()
    {
        var services = new ServiceCollection();
        services.AddAuthorization(ApiAuthorizationPolicies.Configure);
        var provider = services.BuildServiceProvider();

        var options = provider.GetRequiredService<IOptions<AuthorizationOptions>>().Value;

        options.GetPolicy(ApiAuthorizationPolicies.CanReviewVisits).Should().NotBeNull();
        options.GetPolicy(ApiAuthorizationPolicies.CanManageEscalations).Should().NotBeNull();
        options.GetPolicy(ApiAuthorizationPolicies.CanViewEscalations).Should().NotBeNull();
        options.GetPolicy(ApiAuthorizationPolicies.CanViewKpis).Should().NotBeNull();
        options.GetPolicy(ApiAuthorizationPolicies.CanManageUsers).Should().NotBeNull();
        options.GetPolicy(ApiAuthorizationPolicies.CanManageOffices).Should().NotBeNull();
        options.GetPolicy(ApiAuthorizationPolicies.CanManageSites).Should().NotBeNull();
        options.GetPolicy(ApiAuthorizationPolicies.CanViewAnalytics).Should().NotBeNull();
        options.GetPolicy(ApiAuthorizationPolicies.CanViewSites).Should().NotBeNull();
        options.GetPolicy(ApiAuthorizationPolicies.CanViewReports).Should().NotBeNull();
        options.GetPolicy(ApiAuthorizationPolicies.CanViewMaterials).Should().NotBeNull();
    }

    [Theory]
    [InlineData(typeof(VisitsController), ApiAuthorizationPolicies.CanReviewVisits)]
    [InlineData(typeof(EscalationsController), ApiAuthorizationPolicies.CanManageEscalations)]
    [InlineData(typeof(EscalationsController), ApiAuthorizationPolicies.CanViewEscalations)]
    [InlineData(typeof(KpiController), ApiAuthorizationPolicies.CanViewKpis)]
    [InlineData(typeof(UsersController), ApiAuthorizationPolicies.CanManageUsers)]
    [InlineData(typeof(OfficesController), ApiAuthorizationPolicies.CanManageOffices)]
    [InlineData(typeof(SitesController), ApiAuthorizationPolicies.CanManageSites)]
    [InlineData(typeof(AnalyticsController), ApiAuthorizationPolicies.CanViewAnalytics)]
    [InlineData(typeof(SitesController), ApiAuthorizationPolicies.CanViewSites)]
    [InlineData(typeof(ReportsController), ApiAuthorizationPolicies.CanViewReports)]
    [InlineData(typeof(MaterialsController), ApiAuthorizationPolicies.CanViewMaterials)]
    public void Controllers_ShouldReferenceExpectedPolicies(Type controllerType, string policy)
    {
        var methods = controllerType
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Where(m => m.GetCustomAttributes().Any(a => a.GetType().Name.EndsWith("HttpGetAttribute", StringComparison.Ordinal)
                || a.GetType().Name.EndsWith("HttpPostAttribute", StringComparison.Ordinal)
                || a.GetType().Name.EndsWith("HttpPutAttribute", StringComparison.Ordinal)
                || a.GetType().Name.EndsWith("HttpPatchAttribute", StringComparison.Ordinal)
                || a.GetType().Name.EndsWith("HttpDeleteAttribute", StringComparison.Ordinal)))
            .ToArray();

        var allPolicies = controllerType
            .GetCustomAttributes<AuthorizeAttribute>()
            .Concat(methods.SelectMany(m =>
                m.GetCustomAttributes<AuthorizeAttribute>()))
            .Select(a => a.Policy);

        allPolicies.Should().Contain(policy,
            because: $"{controllerType.Name} must enforce {policy}");
    }
}
