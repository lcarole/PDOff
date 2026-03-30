using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace PDOff.Tests.Helpers;

/// <summary>
/// Creates simple test PDF files for integration tests.
/// </summary>
public static class PdfTestHelper
{
    public static string CreateTestPdf(string directory, int pageCount = 1, string? fileName = null)
    {
        fileName ??= $"test_{Guid.NewGuid():N}.pdf";
        var path = Path.Combine(directory, fileName);

        using var writer = new PdfWriter(path);
        using var pdf = new PdfDocument(writer);
        using var doc = new Document(pdf);

        for (int i = 1; i <= pageCount; i++)
        {
            if (i > 1) doc.Add(new AreaBreak());
            doc.Add(new Paragraph($"Page {i}"));
        }

        return path;
    }

    public static int GetPageCount(string pdfPath)
    {
        using var reader = new PdfReader(pdfPath);
        using var doc = new PdfDocument(reader);
        return doc.GetNumberOfPages();
    }

    public static int GetPageRotation(string pdfPath, int pageNumber)
    {
        using var reader = new PdfReader(pdfPath);
        using var doc = new PdfDocument(reader);
        return doc.GetPage(pageNumber).GetRotation();
    }
}
