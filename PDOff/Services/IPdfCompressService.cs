using PDOff.Models;

namespace PDOff.Services;

public enum CompressionLevel
{
    Low,
    Medium,
    High
}

public interface IPdfCompressService
{
    PdfToolResult Compress(string inputPath, string outputPath, CompressionLevel level);
}
