using System;
using System.Collections.Generic;
using System.IO;
using iText.Kernel.Pdf;
using PDOff.Helpers;
using PDOff.Models;

namespace PDOff.Services;

public class PdfSplitService : IPdfSplitService
{
    public PdfToolResult Split(string inputPath, string outputDirectory, SplitMode mode, int everyN = 1, string? pageRange = null)
    {
        if (!File.Exists(inputPath))
            return new PdfToolResult(false, ErrorMessage: string.Format(Lang.Instance["FileNotFound"], inputPath));

        try
        {
            Directory.CreateDirectory(outputDirectory);
            var baseName = Path.GetFileNameWithoutExtension(inputPath);

            using var reader = new PdfReader(inputPath);
            using var srcDoc = new PdfDocument(reader);
            int totalPages = srcDoc.GetNumberOfPages();

            var ranges = GetPageRanges(mode, totalPages, everyN, pageRange);

            int partIndex = 1;
            foreach (var range in ranges)
            {
                var outputPath = Path.Combine(outputDirectory, $"{baseName}_part{partIndex}.pdf");
                using var writer = new PdfWriter(outputPath);
                using var destDoc = new PdfDocument(writer);

                srcDoc.CopyPagesTo(range, destDoc);
                partIndex++;
            }

            return new PdfToolResult(true, outputDirectory);
        }
        catch (Exception ex)
        {
            return new PdfToolResult(false, ErrorMessage: string.Format(Lang.Instance["SplitError"], ex.Message));
        }
    }

    private static List<IList<int>> GetPageRanges(SplitMode mode, int totalPages, int everyN, string? pageRange)
    {
        var ranges = new List<IList<int>>();

        switch (mode)
        {
            case SplitMode.EachPage:
                for (int i = 1; i <= totalPages; i++)
                    ranges.Add(new List<int> { i });
                break;

            case SplitMode.EveryNPages:
                if (everyN < 1) everyN = 1;
                for (int start = 1; start <= totalPages; start += everyN)
                {
                    var pages = new List<int>();
                    for (int p = start; p < start + everyN && p <= totalPages; p++)
                        pages.Add(p);
                    ranges.Add(pages);
                }
                break;

            case SplitMode.PageRange:
                if (string.IsNullOrWhiteSpace(pageRange))
                    throw new ArgumentException(Lang.Instance["SplitPageRangeRequired"]);
                var pages = PageRangeParser.Parse(pageRange, totalPages);
                if (pages.Count == 0)
                    throw new ArgumentException(Lang.Instance["SplitPageRangeEmpty"]);
                ranges.Add(pages);
                break;
        }

        return ranges;
    }
}
