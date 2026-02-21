using FluentAssertions;
using TelecomPm.Application.Services.ExcelParsers;
using Xunit;

namespace TelecomPM.Application.Tests.Services.ExcelParsers;

public class ExcelParsersTests
{
    [Fact]
    public void ExcelDateParser_ShouldParseExcelSerialToUtc()
    {
        var parsed = ExcelDateParser.ParseToUtc(40695);

        parsed.Should().NotBeNull();
        parsed!.Value.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void ExcelDateParser_ShouldParseTextDateToUtc()
    {
        var parsed = ExcelDateParser.ParseToUtc("26/10/2022");

        parsed.Should().Be(new DateTime(2022, 10, 26, 0, 0, 0, DateTimeKind.Utc));
    }

    [Fact]
    public void ExcelDateParser_ShouldReturnNullForEmpty()
    {
        ExcelDateParser.ParseToUtc(null).Should().BeNull();
        ExcelDateParser.ParseToUtc(string.Empty).Should().BeNull();
    }

    [Fact]
    public void CoordinateSplitter_ShouldParseCoordinates()
    {
        var result = CoordinateSplitter.Parse("31.58812,31.026328");

        result.Should().NotBeNull();
        result!.Value.Latitude.Should().Be(31.58812m);
        result.Value.Longitude.Should().Be(31.026328m);
    }

    [Fact]
    public void BatteryFieldParser_ShouldParseSbs170Ah()
    {
        var result = BatteryFieldParser.Parse("SBS 12 V / 170 AH");

        result.Should().BeEquivalentTo(new BatteryFieldParseResult("SBS", 12, 170));
    }

    [Fact]
    public void BatteryFieldParser_ShouldParseDecimalAhAsInteger()
    {
        var result = BatteryFieldParser.Parse("SBS 12 V / 92,40 AH");

        result.Should().BeEquivalentTo(new BatteryFieldParseResult("SBS", 12, 92));
    }

    [Fact]
    public void BatteryFieldParser_ShouldParseBrandOnly()
    {
        var result = BatteryFieldParser.Parse("Marathon");

        result.Should().BeEquivalentTo(new BatteryFieldParseResult("Marathon", null, null));
    }

    [Fact]
    public void BatteryFieldParser_ShouldReturnNullForNotInstalled()
    {
        BatteryFieldParser.Parse("Batteries not installed").Should().BeNull();
    }

    [Fact]
    public void AcUnitsParser_ShouldParseVendorAndHp()
    {
        var result = ACUnitsParser.Parse("Huawei / 0.5 HP");

        result.Should().Be(new AcUnitsParseResult(1, "Huawei", 0.5m));
    }

    [Fact]
    public void AcUnitsParser_ShouldParseMultiUnitHpList()
    {
        var result = ACUnitsParser.Parse("3HP , 3HP");

        result.Should().Be(new AcUnitsParseResult(2, null, 3m));
    }

    [Fact]
    public void AcUnitsParser_ShouldParseZeroGrill()
    {
        var result = ACUnitsParser.Parse("0/Grill");

        result.Should().Be(new AcUnitsParseResult(0, null, null));
    }

    [Fact]
    public void AcUnitsParser_ShouldParseCountOnly()
    {
        var result = ACUnitsParser.Parse("2");

        result.Should().Be(new AcUnitsParseResult(2, null, null));
    }

    [Fact]
    public void AcUnitsParser_ShouldParseNotExistAsZero()
    {
        var result = ACUnitsParser.Parse("Not Exist");

        result.Should().Be(new AcUnitsParseResult(0, null, null));
    }

    [Theory]
    [InlineData("Yes", true)]
    [InlineData("1", true)]
    [InlineData("Exist", true)]
    [InlineData("No", false)]
    [InlineData("0", false)]
    [InlineData("Not Exist", false)]
    [InlineData("NA", null)]
    public void BooleanTextParser_ShouldParseKnownValues(string input, bool? expected)
    {
        var result = BooleanTextParser.ParseNullable(input);

        result.Should().Be(expected);
    }

    [Fact]
    public void BooleanTextParser_ShouldReturnNullForNullOrEmpty()
    {
        BooleanTextParser.ParseNullable(null).Should().BeNull();
        BooleanTextParser.ParseNullable(string.Empty).Should().BeNull();
    }
}
