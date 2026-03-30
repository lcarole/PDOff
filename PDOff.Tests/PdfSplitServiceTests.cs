using PDOff.Models;
using PDOff.Services;
using PDOff.Tests.Helpers;

namespace PDOff.Tests;

public class PdfSplitServiceTests : IDisposable
{
    private readonly PdfSplitService _service = new();
    private readonly string _tempDir;
    private readonly string _outputDir;

    public PdfSplitServiceTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"PDOff_SplitTests_{Guid.NewGuid():N}");
        _outputDir = Path.Combine(_tempDir, "output");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }

    [Fact]
    public void Split_NonExistentFile_ReturnsError()
    {
        var result = _service.Split("/nonexistent/file.pdf", _outputDir, SplitMode.EachPage);

        Assert.False(result.Success);
        Assert.Contains("/nonexistent/file.pdf", result.ErrorMessage);
    }

    [Fact]
    public void Split_EachPage_CreatesOneFilePerPage()
    {
        var input = PdfTestHelper.CreateTestPdf(_tempDir, pageCount: 4);

        var result = _service.Split(input, _outputDir, SplitMode.EachPage);

        Assert.True(result.Success);
        var outputFiles = Directory.GetFiles(_outputDir, "*.pdf");
        Assert.Equal(4, outputFiles.Length);
        foreach (var file in outputFiles)
            Assert.Equal(1, PdfTestHelper.GetPageCount(file));
    }

    [Fact]
    public void Split_EveryNPages_GroupsCorrectly()
    {
        var input = PdfTestHelper.CreateTestPdf(_tempDir, pageCount: 7);

        var result = _service.Split(input, _outputDir, SplitMode.EveryNPages, everyN: 3);

        Assert.True(result.Success);
        var outputFiles = Directory.GetFiles(_outputDir, "*.pdf").OrderBy(f => f).ToArray();
        // 7 pages / 3 = 3 files: (1-3), (4-6), (7)
        Assert.Equal(3, outputFiles.Length);
        Assert.Equal(3, PdfTestHelper.GetPageCount(outputFiles[0]));
        Assert.Equal(3, PdfTestHelper.GetPageCount(outputFiles[1]));
        Assert.Equal(1, PdfTestHelper.GetPageCount(outputFiles[2]));
    }

    [Fact]
    public void Split_EveryNPages_DefaultsTo1WhenNIs0()
    {
        var input = PdfTestHelper.CreateTestPdf(_tempDir, pageCount: 3);

        var result = _service.Split(input, _outputDir, SplitMode.EveryNPages, everyN: 0);

        Assert.True(result.Success);
        var outputFiles = Directory.GetFiles(_outputDir, "*.pdf");
        Assert.Equal(3, outputFiles.Length);
    }

    [Fact]
    public void Split_PageRange_CreatesOneFileWithSelectedPages()
    {
        var input = PdfTestHelper.CreateTestPdf(_tempDir, pageCount: 10);

        var result = _service.Split(input, _outputDir, SplitMode.PageRange, pageRange: "2-4, 7");

        Assert.True(result.Success);
        var outputFiles = Directory.GetFiles(_outputDir, "*.pdf");
        Assert.Single(outputFiles);
        Assert.Equal(4, PdfTestHelper.GetPageCount(outputFiles[0]));
    }

    [Fact]
    public void Split_PageRange_NullRange_ReturnsError()
    {
        var input = PdfTestHelper.CreateTestPdf(_tempDir, pageCount: 5);

        var result = _service.Split(input, _outputDir, SplitMode.PageRange, pageRange: null);

        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
    }

    [Fact]
    public void Split_PageRange_EmptyRange_ReturnsError()
    {
        var input = PdfTestHelper.CreateTestPdf(_tempDir, pageCount: 5);

        var result = _service.Split(input, _outputDir, SplitMode.PageRange, pageRange: "");

        Assert.False(result.Success);
    }

    [Fact]
    public void Split_PageRange_InvalidRange_ReturnsError()
    {
        var input = PdfTestHelper.CreateTestPdf(_tempDir, pageCount: 5);

        var result = _service.Split(input, _outputDir, SplitMode.PageRange, pageRange: "abc");

        Assert.False(result.Success);
    }

    [Fact]
    public void Split_OutputDirectoryIsCreated()
    {
        var input = PdfTestHelper.CreateTestPdf(_tempDir, pageCount: 2);
        var newOutputDir = Path.Combine(_tempDir, "deep", "nested", "output");

        var result = _service.Split(input, newOutputDir, SplitMode.EachPage);

        Assert.True(result.Success);
        Assert.True(Directory.Exists(newOutputDir));
    }

    [Fact]
    public void Split_SinglePage_EachPage_CreatesOneFile()
    {
        var input = PdfTestHelper.CreateTestPdf(_tempDir, pageCount: 1);

        var result = _service.Split(input, _outputDir, SplitMode.EachPage);

        Assert.True(result.Success);
        var outputFiles = Directory.GetFiles(_outputDir, "*.pdf");
        Assert.Single(outputFiles);
    }
}
