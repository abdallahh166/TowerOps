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
    }

    [Theory]
    [InlineData(typeof(VisitsController), ApiAuthorizationPolicies.CanReviewVisits)]
    [InlineData(typeof(EscalationsController), ApiAuthorizationPolicies.CanManageEscalations)]
    [InlineData(typeof(EscalationsController), ApiAuthorizationPolicies.CanViewEscalations)]
    [InlineData(typeof(KpiController), ApiAuthorizationPolicies.CanViewKpis)]
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

        methods
            .SelectMany(m => m.GetCustomAttributes<AuthorizeAttribute>())
            .Select(a => a.Policy)
            .Should()
            .Contain(policy);
    }
}
