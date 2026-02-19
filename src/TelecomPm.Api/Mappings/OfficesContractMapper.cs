namespace TelecomPm.Api.Mappings;

using TelecomPm.Api.Contracts.Offices;
using TelecomPM.Application.Commands.Offices.CreateOffice;
using TelecomPM.Application.Commands.Offices.DeleteOffice;
using TelecomPM.Application.Commands.Offices.UpdateOffice;
using TelecomPM.Application.Commands.Offices.UpdateOfficeContact;
using TelecomPM.Application.Queries.Offices.GetAllOffices;
using TelecomPM.Application.Queries.Offices.GetOfficeById;
using TelecomPM.Application.Queries.Offices.GetOfficeStatistics;
using TelecomPM.Application.Queries.Offices.GetOfficesByRegion;

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

    public static GetAllOfficesQuery ToGetAllQuery()
        => new();

    public static DeleteOfficeCommand ToDeleteCommand(this Guid officeId)
        => new() { OfficeId = officeId };
}
