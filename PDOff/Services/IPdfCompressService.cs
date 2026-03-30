using PDOff.Models;

namespace PDOff.Services;

public interface IPdfCompressService
{
    PdfToolResult Compress(string inputPath, string outputPath, CompressionLevel level);
}
