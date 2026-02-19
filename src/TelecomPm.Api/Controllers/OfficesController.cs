namespace TelecomPm.Api.Controllers;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TelecomPm.Api.Contracts.Offices;
using TelecomPM.Application.Commands.Offices.CreateOffice;
using TelecomPM.Application.Commands.Offices.UpdateOffice;
using TelecomPM.Application.Commands.Offices.UpdateOfficeContact;
using TelecomPM.Application.Commands.Offices.DeleteOffice;
using TelecomPM.Application.Queries.Offices.GetOfficeById;
using TelecomPM.Application.Queries.Offices.GetAllOffices;
using TelecomPM.Application.Queries.Offices.GetOfficesByRegion;
using TelecomPM.Application.Queries.Offices.GetOfficeStatistics;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class OfficesController : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateOfficeRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateOfficeCommand
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

        var result = await Mediator.Send(command, cancellationToken);

        if (result.IsSuccess && result.Value is not null)
        {
            return CreatedAtAction(
                nameof(GetById),
                new { officeId = result.Value.Id },
                result.Value);
        }

        return HandleResult(result);
    }

    [HttpGet("{officeId:guid}")]
    public async Task<IActionResult> GetById(
        Guid officeId,
        CancellationToken cancellationToken)
    {
        var query = new GetOfficeByIdQuery { OfficeId = officeId };
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllOfficesQuery();
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("region/{region}")]
    public async Task<IActionResult> GetByRegion(
        string region,
        CancellationToken cancellationToken)
    {
        var query = new GetOfficesByRegionQuery { Region = region };
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{officeId:guid}/statistics")]
    public async Task<IActionResult> GetStatistics(
        Guid officeId,
        CancellationToken cancellationToken)
    {
        var query = new GetOfficeStatisticsQuery { OfficeId = officeId };
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{officeId:guid}")]
    public async Task<IActionResult> Update(
        Guid officeId,
        [FromBody] UpdateOfficeRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateOfficeCommand
        {
            OfficeId = officeId,
            Name = request.Name,
            Region = request.Region,
            City = request.Address.City,
            Street = request.Address.Street ?? string.Empty,
            BuildingNumber = null,
            PostalCode = null
        };

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{officeId:guid}/contact")]
    public async Task<IActionResult> UpdateContact(
        Guid officeId,
        [FromBody] UpdateOfficeContactRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateOfficeContactCommand
        {
            OfficeId = officeId,
            ContactPerson = request.ContactPerson ?? string.Empty,
            ContactPhone = request.ContactPhone ?? string.Empty,
            ContactEmail = request.ContactEmail ?? string.Empty
        };

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{officeId:guid}")]
    public async Task<IActionResult> Delete(
        Guid officeId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteOfficeCommand { OfficeId = officeId };
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}

