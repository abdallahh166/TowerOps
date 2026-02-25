namespace TowerOps.Api.Mappings;

using TowerOps.Api.Contracts.Offices;
using TowerOps.Application.Commands.Offices.CreateOffice;
using TowerOps.Application.Commands.Offices.DeleteOffice;
using TowerOps.Application.Commands.Offices.UpdateOffice;
using TowerOps.Application.Commands.Offices.UpdateOfficeContact;
using TowerOps.Application.Queries.Offices.GetAllOffices;
using TowerOps.Application.Queries.Offices.GetOfficeById;
using TowerOps.Application.Queries.Offices.GetOfficeStatistics;
using TowerOps.Application.Queries.Offices.GetOfficesByRegion;

public static class OfficesContractMapper
{
    public static CreateOfficeCommand ToCommand(this CreateOfficeRequest request)
        => new()
        {
            Code = request.Code,
            Name = request.Name,
            Region = request.Region,
            City = request.Address.City,
            Street = request.Address.Street ?? string.Empty,
            BuildingNumber = null,
            PostalCode = null,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            ContactPerson = request.ContactPerson,
            ContactPhone = request.ContactPhone,
            ContactEmail = request.ContactEmail
        };

    public static UpdateOfficeCommand ToCommand(this UpdateOfficeRequest request, Guid officeId)
        => new()
        {
            OfficeId = officeId,
            Name = request.Name,
            Region = request.Region,
            City = request.Address.City,
            Street = request.Address.Street ?? string.Empty,
            BuildingNumber = null,
            PostalCode = null
        };

    public static UpdateOfficeContactCommand ToCommand(this UpdateOfficeContactRequest request, Guid officeId)
        => new()
        {
            OfficeId = officeId,
            ContactPerson = request.ContactPerson ?? string.Empty,
            ContactPhone = request.ContactPhone ?? string.Empty,
            ContactEmail = request.ContactEmail ?? string.Empty
        };

    public static GetOfficeByIdQuery ToOfficeByIdQuery(this Guid officeId)
        => new() { OfficeId = officeId };

    public static GetOfficeStatisticsQuery ToOfficeStatisticsQuery(this Guid officeId)
        => new() { OfficeId = officeId };

    public static GetOfficesByRegionQuery ToRegionQuery(this string region)
        => new() { Region = region };

    public static GetAllOfficesQuery ToGetAllQuery(bool? onlyActive, int? pageNumber, int? pageSize)
        => new()
        {
            OnlyActive = onlyActive,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

    public static DeleteOfficeCommand ToDeleteCommand(this Guid officeId)
        => new() { OfficeId = officeId };
}
