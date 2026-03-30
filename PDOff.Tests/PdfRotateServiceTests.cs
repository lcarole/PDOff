using PDOff.Services;
using PDOff.Tests.Helpers;

namespace PDOff.Tests;

public class PdfRotateServiceTests : IDisposable
{
    private readonly PdfRotateService _service = new();
    private readonly string _tempDir;

    public PdfRotateServiceTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"PDOff_RotateTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }

    [Fact]
    public void Rotate_InvalidAngle_ReturnsError()
    {
        var input = PdfTestHelper.CreateTestPdf(_tempDir);
        var output = Path.Combine(_tempDir, "rotated.pdf");

        var result = _service.Rotate(input, output, angleDegrees: 45);

        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
    }

    [Fact]
    public void Rotate_NonExistentFile_ReturnsError()
    {
        var output = Path.Combine(_tempDir, "rotated.pdf");

        var result = _service.Rotate("/nonexistent/file.pdf", output, angleDegrees: 90);

        Assert.False(result.Success);
        Assert.Contains("/nonexistent/file.pdf", result.ErrorMessage);
    }

    [Theory]
    [InlineData(90)]
    [InlineData(180)]
    [InlineData(270)]
    public void Rotate_ValidAngles_Succeeds(int angle)
    {
        var input = PdfTestHelper.CreateTestPdf(_tempDir, pageCount: 2);
        var output = Path.Combine(_tempDir, $"rotated_{angle}.pdf");

        var result = _service.Rotate(input, output, angleDegrees: angle);

        Assert.True(result.Success);
        Assert.Equal(output, result.OutputPath);
        Assert.True(File.Exists(output));
    }

    [Fact]
    public void Rotate_AllPages_RotatesEveryPage()
    {
        var input = PdfTestHelper.CreateTestPdf(_tempDir, pageCount: 3);
        var output = Path.Combine(_tempDir, "rotated.pdf");

        _service.Rotate(input, output, angleDegrees: 90);

        for (int i = 1; i <= 3; i++)
            Assert.Equal(90, PdfTestHelper.GetPageRotation(output, i));
    }

    [Fact]
    public void Rotate_SpecificPages_OnlyRotatesThosePages()
    {
        var input = PdfTestHelper.CreateTestPdf(_tempDir, pageCount: 4);
        var output = Path.Combine(_tempDir, "rotated.pdf");

        _service.Rotate(input, output, angleDegrees: 90, pageNumbers: [1, 3]);

        Assert.Equal(90, PdfTestHelper.GetPageRotation(output, 1));
        Assert.Equal(0, PdfTestHelper.GetPageRotation(output, 2));
        Assert.Equal(90, PdfTestHelper.GetPageRotation(output, 3));
        Assert.Equal(0, PdfTestHelper.GetPageRotation(output, 4));
    }

    [Fact]
    public void Rotate_NegativeAngle_HandledCorrectly()
    {
        var input = PdfTestHelper.CreateTestPdf(_tempDir, pageCount: 1);
        var output = Path.Combine(_tempDir, "rotated.pdf");

        var result = _service.Rotate(input, output, angleDegrees: -90);

        Assert.True(result.Success);
        Assert.Equal(270, PdfTestHelper.GetPageRotation(output, 1));
    }

    [Fact]
    public void Rotate_360Degrees_ResultsInZeroRotation()
    {
        var input = PdfTestHelper.CreateTestPdf(_tempDir, pageCount: 1);
        var output = Path.Combine(_tempDir, "rotated.pdf");

        _service.Rotate(input, output, angleDegrees: 360);

        Assert.Equal(0, PdfTestHelper.GetPageRotation(output, 1));
    }

    [Fact]
    public void Rotate_OutOfRangePageNumbers_AreSkipped()
    {
        var input = PdfTestHelper.CreateTestPdf(_tempDir, pageCount: 2);
        var output = Path.Combine(_tempDir, "rotated.pdf");

        // Page 99 doesn't exist — should be silently skipped
        var result = _service.Rotate(input, output, angleDegrees: 90, pageNumbers: [1, 99]);

        Assert.True(result.Success);
        Assert.Equal(90, PdfTestHelper.GetPageRotation(output, 1));
        Assert.Equal(0, PdfTestHelper.GetPageRotation(output, 2));
    }

    [Fact]
    public void Rotate_EmptyPageList_RotatesAllPages()
    {
        var input = PdfTestHelper.CreateTestPdf(_tempDir, pageCount: 2);
        var output = Path.Combine(_tempDir, "rotated.pdf");

        _service.Rotate(input, output, angleDegrees: 180, pageNumbers: []);

        // Empty list = all pages
        Assert.Equal(180, PdfTestHelper.GetPageRotation(output, 1));
        Assert.Equal(180, PdfTestHelper.GetPageRotation(output, 2));
    }

    [Fact]
    public void Rotate_PreservesPageCount()
    {
        var input = PdfTestHelper.CreateTestPdf(_tempDir, pageCount: 5);
        var output = Path.Combine(_tempDir, "rotated.pdf");

        _service.Rotate(input, output, angleDegrees: 90);

        Assert.Equal(5, PdfTestHelper.GetPageCount(output));
    }
}
