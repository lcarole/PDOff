using System.Globalization;
using PDOff.Converters;

namespace PDOff.Tests;

public class FileSizeConverterTests : IDisposable
{
    private readonly FileSizeConverter _converter = FileSizeConverter.Instance;
    private readonly CultureInfo _originalCulture;

    public FileSizeConverterTests()
    {
        _originalCulture = Thread.CurrentThread.CurrentCulture;
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
    }

    public void Dispose()
    {
        Thread.CurrentThread.CurrentCulture = _originalCulture;
    }

    [Fact]
    public void Convert_Bytes_DisplaysBytes()
    {
        var result = _converter.Convert(500L, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("500 o", result);
    }

    [Fact]
    public void Convert_ZeroBytes_DisplaysZero()
    {
        var result = _converter.Convert(0L, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("0 o", result);
    }

    [Fact]
    public void Convert_Kilobytes_DisplaysKo()
    {
        var result = (string)_converter.Convert(2048L, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("2.0 Ko", result);
    }

    [Fact]
    public void Convert_Megabytes_DisplaysMo()
    {
        long bytes = 5 * 1024 * 1024;
        var result = (string)_converter.Convert(bytes, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("5.0 Mo", result);
    }

    [Fact]
    public void Convert_Gigabytes_DisplaysGo()
    {
        long bytes = 2L * 1024 * 1024 * 1024;
        var result = (string)_converter.Convert(bytes, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("2.00 Go", result);
    }

    [Fact]
    public void Convert_JustBelowKilobyte_DisplaysBytes()
    {
        var result = _converter.Convert(1023L, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("1023 o", result);
    }

    [Fact]
    public void Convert_ExactlyOneKb_DisplaysKo()
    {
        var result = (string)_converter.Convert(1024L, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("1.0 Ko", result);
    }

    [Fact]
    public void Convert_NullValue_ReturnsDash()
    {
        var result = _converter.Convert(null, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("—", result);
    }

    [Fact]
    public void Convert_NonLongValue_ReturnsDash()
    {
        var result = _converter.Convert("not a number", typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("—", result);
    }

    [Fact]
    public void Convert_IntValue_ReturnsDash()
    {
        // int is not long — should fall through to dash
        var result = _converter.Convert(500, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("—", result);
    }

    [Fact]
    public void ConvertBack_ThrowsNotSupportedException()
    {
        Assert.Throws<NotSupportedException>(() =>
            _converter.ConvertBack("500 o", typeof(long), null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void Instance_IsSingleton()
    {
        Assert.Same(FileSizeConverter.Instance, FileSizeConverter.Instance);
    }
}
