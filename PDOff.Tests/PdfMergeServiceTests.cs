using PDOff.Services;
using PDOff.Tests.Helpers;

namespace PDOff.Tests;

public class PdfMergeServiceTests : IDisposable
{
    private readonly PdfMergeService _service = new();
    private readonly string _tempDir;

    public PdfMergeServiceTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"PDOff_MergeTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }

    [Fact]
    public void Merge_LessThan2Files_ReturnsError()
    {
        var pdf = PdfTestHelper.CreateTestPdf(_tempDir);
        var output = Path.Combine(_tempDir, "merged.pdf");

        var result = _service.Merge([pdf], output);

        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
    }

    [Fact]
    public void Merge_EmptyList_ReturnsError()
    {
        var output = Path.Combine(_tempDir, "merged.pdf");

        var result = _service.Merge([], output);

        Assert.False(result.Success);
    }

    [Fact]
    public void Merge_NonExistentFile_ReturnsFileNotFoundError()
    {
        var validPdf = PdfTestHelper.CreateTestPdf(_tempDir);
        var output = Path.Combine(_tempDir, "merged.pdf");

        var result = _service.Merge([validPdf, "/nonexistent/file.pdf"], output);

        Assert.False(result.Success);
        Assert.Contains("/nonexistent/file.pdf", result.ErrorMessage);
    }

    [Fact]
    public void Merge_TwoValidPdfs_Succeeds()
    {
        var pdf1 = PdfTestHelper.CreateTestPdf(_tempDir, pageCount: 2, fileName: "a.pdf");
        var pdf2 = PdfTestHelper.CreateTestPdf(_tempDir, pageCount: 3, fileName: "b.pdf");
        var output = Path.Combine(_tempDir, "merged.pdf");

        var result = _service.Merge([pdf1, pdf2], output);

        Assert.True(result.Success);
        Assert.Equal(output, result.OutputPath);
        Assert.True(File.Exists(output));
        Assert.Equal(5, PdfTestHelper.GetPageCount(output));
    }

    [Fact]
    public void Merge_ThreeValidPdfs_Succeeds()
    {
        var pdf1 = PdfTestHelper.CreateTestPdf(_tempDir, pageCount: 1, fileName: "a.pdf");
        var pdf2 = PdfTestHelper.CreateTestPdf(_tempDir, pageCount: 1, fileName: "b.pdf");
        var pdf3 = PdfTestHelper.CreateTestPdf(_tempDir, pageCount: 1, fileName: "c.pdf");
        var output = Path.Combine(_tempDir, "merged.pdf");

        var result = _service.Merge([pdf1, pdf2, pdf3], output);

        Assert.True(result.Success);
        Assert.Equal(3, PdfTestHelper.GetPageCount(output));
    }

    [Fact]
    public void Merge_OutputFileIsValidPdf()
    {
        var pdf1 = PdfTestHelper.CreateTestPdf(_tempDir, fileName: "a.pdf");
        var pdf2 = PdfTestHelper.CreateTestPdf(_tempDir, fileName: "b.pdf");
        var output = Path.Combine(_tempDir, "merged.pdf");

        _service.Merge([pdf1, pdf2], output);

        // Verify we can open the output as a valid PDF
        var pageCount = PdfTestHelper.GetPageCount(output);
        Assert.True(pageCount > 0);
    }
}
