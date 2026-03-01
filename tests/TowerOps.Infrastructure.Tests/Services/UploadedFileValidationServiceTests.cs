using FluentAssertions;
using TowerOps.Infrastructure.Services;
using Xunit;

namespace TowerOps.Infrastructure.Tests.Services;

public class UploadedFileValidationServiceTests
{
    private readonly UploadedFileValidationService _sut = new();

    [Fact]
    public async Task ValidateAsync_ShouldPass_ForJpegWithMatchingSignature()
    {
        await using var stream = new MemoryStream(new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x10 });

        var result = await _sut.ValidateAsync("photo.jpg", stream, CancellationToken.None);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_ShouldFail_WhenSignatureDoesNotMatchExtension()
    {
        // PDF header but .png extension.
        await using var stream = new MemoryStream(new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D });

        var result = await _sut.ValidateAsync("evidence.png", stream, CancellationToken.None);

        result.IsValid.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.ToLowerInvariant().Should().Contain("signature");
    }

    [Fact]
    public async Task ValidateAsync_ShouldPass_ForPdfWithMatchingSignature()
    {
        await using var stream = new MemoryStream(new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D, 0x31, 0x2E, 0x37 });

        var result = await _sut.ValidateAsync("evidence.pdf", stream, CancellationToken.None);

        result.IsValid.Should().BeTrue();
    }
}
