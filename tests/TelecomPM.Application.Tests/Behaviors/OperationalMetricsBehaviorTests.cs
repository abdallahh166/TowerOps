using FluentAssertions;
using MediatR;
using Moq;
using TelecomPM.Application.Common;
using TelecomPM.Application.Common.Behaviors;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Application.DTOs.Sites;
using TelecomPM.Application.DTOs.Sync;
using Xunit;

namespace TelecomPM.Application.Tests.Behaviors;

public class OperationalMetricsBehaviorTests
{
    private sealed record ImportSiteAssetsCommandForTest : ICommand<ImportSiteDataResult>;
    private sealed record ProcessSyncBatchCommandForTest : ICommand<SyncResultDto>;
    private sealed record NonTrackedQueryForTest : IQuery<string>;

    [Fact]
    public async Task Handle_ImportSuccess_ShouldRecordImportMetrics()
    {
        var metrics = new Mock<IOperationalMetrics>();
        var behavior = new OperationalMetricsBehavior<ImportSiteAssetsCommandForTest, Result<ImportSiteDataResult>>(metrics.Object);
        var request = new ImportSiteAssetsCommandForTest();

        var response = await behavior.Handle(
            request,
            () => Task.FromResult(Result.Success(new ImportSiteDataResult
            {
                ImportedCount = 10,
                SkippedCount = 2,
                Errors = new List<string> { "e1", "e2", "e3" }
            })),
            CancellationToken.None);

        response.IsSuccess.Should().BeTrue();
        metrics.Verify(m => m.RecordImport(
                nameof(ImportSiteAssetsCommandForTest),
                "success",
                10,
                2,
                3,
                It.IsAny<double>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_SyncFailureResult_ShouldRecordFailureMetrics()
    {
        var metrics = new Mock<IOperationalMetrics>();
        var behavior = new OperationalMetricsBehavior<ProcessSyncBatchCommandForTest, Result<SyncResultDto>>(metrics.Object);
        var request = new ProcessSyncBatchCommandForTest();

        var response = await behavior.Handle(
            request,
            () => Task.FromResult(Result.Failure<SyncResultDto>("failed")),
            CancellationToken.None);

        response.IsFailure.Should().BeTrue();
        metrics.Verify(m => m.RecordSyncBatch(
                nameof(ProcessSyncBatchCommandForTest),
                "failure",
                0,
                0,
                0,
                0,
                It.IsAny<double>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ImportException_ShouldRecordExceptionMetrics()
    {
        var metrics = new Mock<IOperationalMetrics>();
        var behavior = new OperationalMetricsBehavior<ImportSiteAssetsCommandForTest, Result<ImportSiteDataResult>>(metrics.Object);
        var request = new ImportSiteAssetsCommandForTest();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            behavior.Handle(
                request,
                () => throw new InvalidOperationException("boom"),
                CancellationToken.None));

        metrics.Verify(m => m.RecordImport(
                nameof(ImportSiteAssetsCommandForTest),
                "exception",
                0,
                0,
                0,
                It.IsAny<double>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_NonTrackedRequest_ShouldNotRecordMetrics()
    {
        var metrics = new Mock<IOperationalMetrics>();
        var behavior = new OperationalMetricsBehavior<NonTrackedQueryForTest, Result<string>>(metrics.Object);
        var request = new NonTrackedQueryForTest();

        var response = await behavior.Handle(
            request,
            () => Task.FromResult(Result.Success("ok")),
            CancellationToken.None);

        response.IsSuccess.Should().BeTrue();
        metrics.VerifyNoOtherCalls();
    }
}
