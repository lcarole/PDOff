using PDOff.Models;

namespace PDOff.Services;

public enum SplitMode
{
    EachPage,
    EveryNPages,
    PageRange
}

public interface IPdfSplitService
{
    PdfToolResult Split(string inputPath, string outputDirectory, SplitMode mode, int everyN = 1, string? pageRange = null);
}
