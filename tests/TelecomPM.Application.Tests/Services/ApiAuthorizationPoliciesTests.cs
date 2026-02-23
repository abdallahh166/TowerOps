using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TelecomPm.Api.Controllers;
using TelecomPM.Api.Authorization;
using TelecomPM.Application.Security;
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

        options.GetPolicy(ApiAuthorizationPolicies.CanManageVisits).Should().NotBeNull();
        options.GetPolicy(ApiAuthorizationPolicies.CanViewVisits).Should().NotBeNull();
        options.GetPolicy(ApiAuthorizationPolicies.CanReviewVisits).Should().NotBeNull();
        options.GetPolicy(ApiAuthorizationPolicies.CanManageEscalations).Should().NotBeNull();
        options.GetPolicy(ApiAuthorizationPolicies.CanViewEscalations).Should().NotBeNull();
        options.GetPolicy(ApiAuthorizationPolicies.CanViewKpis).Should().NotBeNull();
        options.GetPolicy(ApiAuthorizationPolicies.CanManageUsers).Should().NotBeNull();
        options.GetPolicy(ApiAuthorizationPolicies.CanViewUsers).Should().NotBeNull();
        options.GetPolicy(ApiAuthorizationPolicies.CanManageOffices).Should().NotBeNull();
        options.GetPolicy(ApiAuthorizationPolicies.CanManageSites).Should().NotBeNull();
        options.GetPolicy(ApiAuthorizationPolicies.CanViewAnalytics).Should().NotBeNull();
        options.GetPolicy(ApiAuthorizationPolicies.CanViewSites).Should().NotBeNull();
        options.GetPolicy(ApiAuthorizationPolicies.CanViewReports).Should().NotBeNull();
        options.GetPolicy(ApiAuthorizationPolicies.CanViewMaterials).Should().NotBeNull();
        options.GetPolicy(ApiAuthorizationPolicies.CanManageMaterials).Should().NotBeNull();
        options.GetPolicy(ApiAuthorizationPolicies.CanManageSettings).Should().NotBeNull();
        options.GetPolicy(ApiAuthorizationPolicies.CanViewPortal).Should().NotBeNull();
    }

    [Fact]
    public async Task CanManageSettingsPolicy_ShouldRequireSettingsEditPermission()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAuthorization(ApiAuthorizationPolicies.Configure);
        var provider = services.BuildServiceProvider();

        var options = provider.GetRequiredService<IOptions<AuthorizationOptions>>().Value;
        var policy = options.GetPolicy(ApiAuthorizationPolicies.CanManageSettings);

        policy.Should().NotBeNull();

        var authService = provider.GetRequiredService<IAuthorizationService>();

        var allowedPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(PermissionConstants.ClaimType, PermissionConstants.SettingsEdit)
        }, "test"));

        var deniedPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(PermissionConstants.ClaimType, PermissionConstants.SettingsView)
        }, "test"));

        var allowed = await authService.AuthorizeAsync(allowedPrincipal, policy!);
        var denied = await authService.AuthorizeAsync(deniedPrincipal, policy!);

        allowed.Succeeded.Should().BeTrue();
        denied.Succeeded.Should().BeFalse();
    }

    [Fact]
    public async Task CanManageOfficesPolicy_ShouldRequireOfficesManagePermission()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAuthorization(ApiAuthorizationPolicies.Configure);
        var provider = services.BuildServiceProvider();

        var options = provider.GetRequiredService<IOptions<AuthorizationOptions>>().Value;
        var policy = options.GetPolicy(ApiAuthorizationPolicies.CanManageOffices);
        policy.Should().NotBeNull();

        var authService = provider.GetRequiredService<IAuthorizationService>();

        var allowedPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(PermissionConstants.ClaimType, PermissionConstants.OfficesManage)
        }, "test"));

        var deniedPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(PermissionConstants.ClaimType, PermissionConstants.SettingsEdit)
        }, "test"));

        var allowed = await authService.AuthorizeAsync(allowedPrincipal, policy!);
        var denied = await authService.AuthorizeAsync(deniedPrincipal, policy!);

        allowed.Succeeded.Should().BeTrue();
        denied.Succeeded.Should().BeFalse();
    }

    [Fact]
    public async Task CanViewPortalPolicy_ShouldDenyEngineerAndAllowPortalUser()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAuthorization(ApiAuthorizationPolicies.Configure);
        var provider = services.BuildServiceProvider();

        var options = provider.GetRequiredService<IOptions<AuthorizationOptions>>().Value;
        var policy = options.GetPolicy(ApiAuthorizationPolicies.CanViewPortal);
        policy.Should().NotBeNull();

        var authService = provider.GetRequiredService<IAuthorizationService>();

        var engineerPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(PermissionConstants.ClaimType, PermissionConstants.VisitsStart)
        }, "test"));

        var portalPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(PermissionConstants.ClaimType, PermissionConstants.PortalViewSites)
        }, "test"));

        var denied = await authService.AuthorizeAsync(engineerPrincipal, policy!);
        var allowed = await authService.AuthorizeAsync(portalPrincipal, policy!);

        denied.Succeeded.Should().BeFalse();
        allowed.Succeeded.Should().BeTrue();
    }

    [Fact]
    public async Task CanViewSitesPolicy_ShouldDenyPortalOnlyUser()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAuthorization(ApiAuthorizationPolicies.Configure);
        var provider = services.BuildServiceProvider();

        var options = provider.GetRequiredService<IOptions<AuthorizationOptions>>().Value;
        var policy = options.GetPolicy(ApiAuthorizationPolicies.CanViewSites);
        policy.Should().NotBeNull();

        var authService = provider.GetRequiredService<IAuthorizationService>();

        var portalPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(PermissionConstants.ClaimType, PermissionConstants.PortalViewSites)
        }, "test"));

        var result = await authService.AuthorizeAsync(portalPrincipal, policy!);
        result.Succeeded.Should().BeFalse();
    }

    [Theory]
    [InlineData(typeof(VisitsController), ApiAuthorizationPolicies.CanManageVisits)]
    [InlineData(typeof(VisitsController), ApiAuthorizationPolicies.CanViewVisits)]
    [InlineData(typeof(VisitsController), ApiAuthorizationPolicies.CanReviewVisits)]
    [InlineData(typeof(SyncController), ApiAuthorizationPolicies.CanManageVisits)]
    [InlineData(typeof(EscalationsController), ApiAuthorizationPolicies.CanManageEscalations)]
    [InlineData(typeof(KpiController), ApiAuthorizationPolicies.CanViewKpis)]
    [InlineData(typeof(UsersController), ApiAuthorizationPolicies.CanManageUsers)]
    [InlineData(typeof(UsersController), ApiAuthorizationPolicies.CanViewUsers)]
    [InlineData(typeof(OfficesController), ApiAuthorizationPolicies.CanManageOffices)]
    [InlineData(typeof(SitesController), ApiAuthorizationPolicies.CanManageSites)]
    [InlineData(typeof(DailyPlansController), ApiAuthorizationPolicies.CanManageSites)]
    [InlineData(typeof(AssetsController), ApiAuthorizationPolicies.CanManageSites)]
    [InlineData(typeof(AnalyticsController), ApiAuthorizationPolicies.CanViewAnalytics)]
    [InlineData(typeof(SitesController), ApiAuthorizationPolicies.CanViewSites)]
    [InlineData(typeof(AssetsController), ApiAuthorizationPolicies.CanViewSites)]
    [InlineData(typeof(ReportsController), ApiAuthorizationPolicies.CanViewReports)]
    [InlineData(typeof(MaterialsController), ApiAuthorizationPolicies.CanViewMaterials)]
    [InlineData(typeof(MaterialsController), ApiAuthorizationPolicies.CanManageMaterials)]
    [InlineData(typeof(SettingsController), ApiAuthorizationPolicies.CanManageSettings)]
    [InlineData(typeof(RolesController), ApiAuthorizationPolicies.CanManageSettings)]
    [InlineData(typeof(ClientPortalController), ApiAuthorizationPolicies.CanViewPortal)]
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
