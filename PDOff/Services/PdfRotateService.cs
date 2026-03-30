using System;
using System.Collections.Generic;
using System.IO;
using iText.Kernel.Pdf;
using PDOff.Models;

namespace PDOff.Services;

public class PdfRotateService : IPdfRotateService
{
    public PdfToolResult Rotate(string inputPath, string outputPath, int angleDegrees, IReadOnlyList<int>? pageNumbers = null)
    {
        if (angleDegrees % 90 != 0)
            return new PdfToolResult(false, ErrorMessage: Lang.Instance["RotateAngleError"]);

        if (!File.Exists(inputPath))
            return new PdfToolResult(false, ErrorMessage: string.Format(Lang.Instance["FileNotFound"], inputPath));

        try
        {
            using var reader = new PdfReader(inputPath);
            using var writer = new PdfWriter(outputPath);
            using var doc = new PdfDocument(reader, writer);

            int total = doc.GetNumberOfPages();
            var targets = (pageNumbers is { Count: > 0 })
                ? pageNumbers
                : AllPages(total);

            foreach (var pageNum in targets)
            {
                if (pageNum < 1 || pageNum > total) continue;
                var page = doc.GetPage(pageNum);
                int current = page.GetRotation();
                page.SetRotation(((current + angleDegrees) % 360 + 360) % 360);
            }

            return new PdfToolResult(true, outputPath);
        }
        catch (Exception ex)
        {
            TryDeleteFile(outputPath);
            return new PdfToolResult(false, ErrorMessage: string.Format(Lang.Instance["RotateError"], ex.Message));
        }
    }

    private static IReadOnlyList<int> AllPages(int total)
    {
        var list = new List<int>(total);
        for (int i = 1; i <= total; i++) list.Add(i);
        return list;
    }

    private static void TryDeleteFile(string path)
    {
        try { if (File.Exists(path)) File.Delete(path); } catch { }
    }
}
