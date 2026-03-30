using PDOff.Models;
using PDOff.Services;
using PDOff.Tests.Helpers;

namespace PDOff.Tests;

public class PdfCompressServiceTests : IDisposable
{
    private readonly PdfCompressService _service = new();
    private readonly string _tempDir;

    public PdfCompressServiceTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"PDOff_CompressTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }

    [Fact]
    public void Compress_NonExistentFile_ReturnsError()
    {
        var output = Path.Combine(_tempDir, "compressed.pdf");

        var result = _service.Compress("/nonexistent/file.pdf", output, CompressionLevel.Medium);

        Assert.False(result.Success);
        Assert.Contains("/nonexistent/file.pdf", result.ErrorMessage);
    }

    [Theory]
    [InlineData(CompressionLevel.Low)]
    [InlineData(CompressionLevel.Medium)]
    [InlineData(CompressionLevel.High)]
    public void Compress_AllLevels_Succeeds(CompressionLevel level)
    {
        var input = PdfTestHelper.CreateTestPdf(_tempDir, pageCount: 3);
        var output = Path.Combine(_tempDir, $"compressed_{level}.pdf");

        var result = _service.Compress(input, output, level);

        Assert.True(result.Success);
        Assert.Equal(output, result.OutputPath);
        Assert.True(File.Exists(output));
    }

    [Theory]
    [InlineData(CompressionLevel.Low)]
    [InlineData(CompressionLevel.Medium)]
    [InlineData(CompressionLevel.High)]
    public void Compress_OutputIsValidPdf(CompressionLevel level)
    {
        var input = PdfTestHelper.CreateTestPdf(_tempDir, pageCount: 2);
        var output = Path.Combine(_tempDir, $"valid_{level}.pdf");

        _service.Compress(input, output, level);

        var pageCount = PdfTestHelper.GetPageCount(output);
        Assert.Equal(2, pageCount);
    }

    [Fact]
    public void Compress_PreservesPageCount()
    {
        var input = PdfTestHelper.CreateTestPdf(_tempDir, pageCount: 5);
        var output = Path.Combine(_tempDir, "compressed.pdf");

        _service.Compress(input, output, CompressionLevel.High);

        Assert.Equal(5, PdfTestHelper.GetPageCount(output));
    }

    [Fact]
    public void Compress_MediumAndHigh_ClearsCreator()
    {
        var input = PdfTestHelper.CreateTestPdf(_tempDir, pageCount: 1);
        var output = Path.Combine(_tempDir, "compressed.pdf");

        _service.Compress(input, output, CompressionLevel.High);

        using var reader = new iText.Kernel.Pdf.PdfReader(output);
        using var doc = new iText.Kernel.Pdf.PdfDocument(reader);
        var info = doc.GetDocumentInfo();
        Assert.Equal("", info.GetCreator());
        // Note: iText AGPL always stamps its own Producer — not clearable
    }
}
