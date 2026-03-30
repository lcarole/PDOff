using System;
using System.Collections.Generic;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using PDOff.Models;

namespace PDOff.Services;

public class PdfMergeService : IPdfMergeService
{
    public PdfToolResult Merge(IReadOnlyList<string> inputPaths, string outputPath)
    {
        if (inputPaths.Count < 2)
            return new PdfToolResult(false, ErrorMessage: Lang.Instance["MergeMinFiles"]);

        foreach (var path in inputPaths)
        {
            if (!File.Exists(path))
                return new PdfToolResult(false, ErrorMessage: string.Format(Lang.Instance["FileNotFound"], path));
        }

        try
        {
            using var writer = new PdfWriter(outputPath);
            using var pdfDoc = new PdfDocument(writer);
            var merger = new PdfMerger(pdfDoc);

            foreach (var path in inputPaths)
            {
                using var reader = new PdfReader(path);
                using var srcDoc = new PdfDocument(reader);
                merger.Merge(srcDoc, 1, srcDoc.GetNumberOfPages());
            }

            return new PdfToolResult(true, outputPath);
        }
        catch (Exception ex)
        {
            TryDeleteFile(outputPath);
            return new PdfToolResult(false, ErrorMessage: string.Format(Lang.Instance["MergeError"], ex.Message));
        }
    }

    private static void TryDeleteFile(string path)
    {
        try { if (File.Exists(path)) File.Delete(path); } catch { }
    }
}
