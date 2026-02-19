using AutoMapper;
using FluentAssertions;
using Moq;
using TelecomPM.Application.Commands.Visits.AddChecklistItem;
using TelecomPM.Application.DTOs.Visits;
using TelecomPM.Application.Services;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using Xunit;


namespace TelecomPM.Application.Tests.Commands.Visits;

public class AddChecklistItemCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldAddChecklistItem()
    {
        var visit = Visit.Create(
            "V-3001",
            Guid.NewGuid(),
            "S-TNT-300",
            "Site 300",
            Guid.NewGuid(),
            "Engineer X",
            DateTime.UtcNow,
            VisitType.PreventiveMaintenance);

        var visitRepo = new Mock<IVisitRepository>();
        visitRepo.Setup(r => r.GetByIdAsync(visit.Id, It.IsAny<CancellationToken>())).ReturnsAsync(visit);

        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mock<IMapper>();
        mapper.Setup(m => m.Map<VisitChecklistDto>(It.IsAny<VisitChecklist>()))
            .Returns((VisitChecklist source) => new VisitChecklistDto
            {
                Id = source.Id,
                Category = source.Category,
                ItemName = source.ItemName,
                Description = source.Description,
                Status = source.Status,
                IsMandatory = source.IsMandatory,
                Notes = source.Notes,
                CompletedAt = source.CompletedAt
            });

        var mutationService = new EditableVisitMutationService(visitRepo.Object, unitOfWork.Object);
        var handler = new AddChecklistItemCommandHandler(mutationService, mapper.Object);
        var command = new AddChecklistItemCommand
        {
            VisitId = visit.Id,
            Category = "Power",
            ItemName = "Battery Check",
            Description = "Verify battery status",
            IsMandatory = true
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Category.Should().Be("Power");
        result.Value.ItemName.Should().Be("Battery Check");
        visit.Checklists.Should().HaveCount(1);
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
