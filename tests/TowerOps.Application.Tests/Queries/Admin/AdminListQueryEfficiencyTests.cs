using AutoMapper;
using FluentAssertions;
using Moq;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Application.Mappings;
using TowerOps.Application.Queries.Offices.GetAllOffices;
using TowerOps.Application.Queries.Roles.GetAllApplicationRoles;
using TowerOps.Application.Queries.Settings.GetAllSystemSettings;
using TowerOps.Domain.Entities.ApplicationRoles;
using TowerOps.Domain.Entities.Offices;
using TowerOps.Domain.Entities.SystemSettings;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.Specifications;
using TowerOps.Domain.ValueObjects;
using Xunit;

namespace TowerOps.Application.Tests.Queries.Admin;

public class AdminListQueryEfficiencyTests
{
    private static readonly IMapper Mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>())
        .CreateMapper();

    [Fact]
    public async Task GetAllOffices_ShouldUseFindAsNoTrackingAndClampPagination()
    {
        var offices = Enumerable.Range(1, 300)
            .Select(i => Office.Create(
                $"{(char)('A' + (i % 26))}{(char)('A' + ((i / 26) % 26))}{(char)('A' + ((i / (26 * 26)) % 26))}",
                $"Office {i}",
                "Region",
                Address.Create("Street", "City", "Region")))
            .ToList();

        var officeRepository = new Mock<IOfficeRepository>();
        officeRepository
            .Setup(r => r.FindAsNoTrackingAsync(It.IsAny<ISpecification<Office>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ISpecification<Office> spec, CancellationToken _) => Apply(spec, offices).ToList());

        var sut = new GetAllOfficesQueryHandler(officeRepository.Object, Mapper);

        var result = await sut.Handle(new GetAllOfficesQuery
        {
            OnlyActive = true,
            PageNumber = 0,
            PageSize = 999
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Count.Should().BeLessThanOrEqualTo(200);

        officeRepository.Verify(r => r.FindAsNoTrackingAsync(It.IsAny<ISpecification<Office>>(), It.IsAny<CancellationToken>()), Times.Once);
        officeRepository.Verify(r => r.GetAllAsNoTrackingAsync(It.IsAny<CancellationToken>()), Times.Never);
        officeRepository.Verify(r => r.GetActiveOfficesAsNoTrackingAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetAllApplicationRoles_ShouldUseFindAsNoTrackingAndClampPagination()
    {
        var roles = Enumerable.Range(1, 250)
            .Select(i => ApplicationRole.Create(
                $"Role{i:D3}",
                $"Role {i}",
                null,
                isSystem: false,
                isActive: true,
                permissions: new[] { "sites.view" }))
            .ToList();

        var roleRepository = new Mock<IApplicationRoleRepository>();
        roleRepository
            .Setup(r => r.FindAsNoTrackingAsync(It.IsAny<ISpecification<ApplicationRole>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ISpecification<ApplicationRole> spec, CancellationToken _) => Apply(spec, roles).ToList());

        var sut = new GetAllApplicationRolesQueryHandler(roleRepository.Object);

        var result = await sut.Handle(new GetAllApplicationRolesQuery
        {
            PageNumber = 1,
            PageSize = 500
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Count.Should().BeLessThanOrEqualTo(200);

        roleRepository.Verify(r => r.FindAsNoTrackingAsync(It.IsAny<ISpecification<ApplicationRole>>(), It.IsAny<CancellationToken>()), Times.Once);
        roleRepository.Verify(r => r.GetAllAsNoTrackingAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetAllSystemSettings_ShouldUseFindAsNoTrackingAndMaskEncryptedValues()
    {
        var settings = Enumerable.Range(1, 240)
            .Select(i => SystemSetting.Create(
                key: $"SLA:P{i}:ResponseMinutes",
                value: (i * 10).ToString(),
                group: "SLA",
                dataType: "int",
                description: null,
                isEncrypted: false,
                updatedBy: "tester"))
            .ToList();

        settings.Add(SystemSetting.Create(
            key: "Notifications:Twilio:AuthToken",
            value: "encrypted",
            group: "Notifications",
            dataType: "secret",
            description: null,
            isEncrypted: true,
            updatedBy: "tester"));

        var settingsRepository = new Mock<ISystemSettingsRepository>();
        settingsRepository
            .Setup(r => r.FindAsNoTrackingAsync(It.IsAny<ISpecification<SystemSetting>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ISpecification<SystemSetting> spec, CancellationToken _) => Apply(spec, settings).ToList());

        var encryptionService = new Mock<ISettingsEncryptionService>();
        var sut = new GetAllSystemSettingsQueryHandler(settingsRepository.Object, encryptionService.Object);

        var result = await sut.Handle(new GetAllSystemSettingsQuery
        {
            PageNumber = 1,
            PageSize = 999,
            MaskEncryptedValues = true
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Count.Should().BeLessThanOrEqualTo(200);
        result.Value.Where(s => s.IsEncrypted).Select(s => s.Value).Should().OnlyContain(v => v == "***");

        settingsRepository.Verify(r => r.FindAsNoTrackingAsync(It.IsAny<ISpecification<SystemSetting>>(), It.IsAny<CancellationToken>()), Times.Once);
        settingsRepository.Verify(r => r.GetAllAsNoTrackingAsync(It.IsAny<CancellationToken>()), Times.Never);
        encryptionService.Verify(e => e.Decrypt(It.IsAny<string>()), Times.Never);
    }

    private static IEnumerable<T> Apply<T>(ISpecification<T> specification, IEnumerable<T> source)
    {
        IQueryable<T> query = source.AsQueryable();

        if (specification.Criteria is not null)
        {
            query = query.Where(specification.Criteria);
        }

        IOrderedQueryable<T>? ordered = null;
        if (specification.OrderBy is not null)
        {
            ordered = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending is not null)
        {
            ordered = query.OrderByDescending(specification.OrderByDescending);
        }

        if (ordered is not null)
        {
            foreach (var then in specification.ThenBy)
            {
                ordered = ordered.ThenBy(then);
            }

            foreach (var thenDesc in specification.ThenByDescending)
            {
                ordered = ordered.ThenByDescending(thenDesc);
            }

            query = ordered;
        }

        if (specification.IsPagingEnabled)
        {
            query = query.Skip(specification.Skip).Take(specification.Take);
        }

        return query.ToList();
    }
}
