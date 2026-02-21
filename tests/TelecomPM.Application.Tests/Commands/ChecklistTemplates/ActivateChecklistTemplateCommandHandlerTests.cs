using FluentAssertions;
using Moq;
using TelecomPM.Application.Commands.ChecklistTemplates.ActivateChecklistTemplate;
using TelecomPM.Domain.Entities.ChecklistTemplates;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using Xunit;

namespace TelecomPM.Application.Tests.Commands.ChecklistTemplates;

public class ActivateChecklistTemplateCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldSupersedePreviousActiveTemplate_AndActivateRequestedTemplate()
    {
        var currentActiveTemplate = ChecklistTemplate.Create(
            VisitType.BM,
            "v1.0",
            DateTime.UtcNow.AddDays(-10),
            "seed");
        currentActiveTemplate.Activate("manager1");

        var nextTemplate = ChecklistTemplate.Create(
            VisitType.BM,
            "v1.1",
            DateTime.UtcNow,
            "seed");

        var repository = new Mock<IChecklistTemplateRepository>();
        repository
            .Setup(r => r.GetByIdAsync(nextTemplate.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(nextTemplate);
        repository
            .Setup(r => r.GetActiveByVisitTypeAsync(VisitType.BM, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentActiveTemplate);

        var unitOfWork = new Mock<IUnitOfWork>();
        var handler = new ActivateChecklistTemplateCommandHandler(repository.Object, unitOfWork.Object);

        var result = await handler.Handle(new ActivateChecklistTemplateCommand
        {
            TemplateId = nextTemplate.Id,
            ApprovedBy = "manager2"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        currentActiveTemplate.IsActive.Should().BeFalse();
        currentActiveTemplate.EffectiveToUtc.Should().NotBeNull();

        nextTemplate.IsActive.Should().BeTrue();
        nextTemplate.ApprovedBy.Should().Be("manager2");
        nextTemplate.ApprovedAtUtc.Should().NotBeNull();

        repository.Verify(r => r.UpdateAsync(currentActiveTemplate, It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(r => r.UpdateAsync(nextTemplate, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
