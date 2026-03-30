using System.Collections.Generic;
using PDOff.Models;

namespace PDOff.Services;

public interface IPdfMergeService
{
    PdfToolResult Merge(IReadOnlyList<string> inputPaths, string outputPath);
}
