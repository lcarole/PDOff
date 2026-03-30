using System.Collections.Generic;
using PDFree.Models;

namespace PDFree.Services;

public interface IPdfRotateService
{
    /// <summary>
    /// Rotates specified pages of a PDF by the given angle (90, 180, 270).
    /// If pageNumbers is empty, rotates all pages.
    /// </summary>
    PdfToolResult Rotate(string inputPath, string outputPath, int angleDegrees, IReadOnlyList<int>? pageNumbers = null);
}
