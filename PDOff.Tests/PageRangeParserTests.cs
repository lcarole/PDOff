using PDOff.Helpers;

namespace PDOff.Tests;

public class PageRangeParserTests
{
    [Fact]
    public void Parse_SinglePage_ReturnsSinglePage()
    {
        var result = PageRangeParser.Parse("5");
        Assert.Equal([5], result);
    }

    [Fact]
    public void Parse_Range_ReturnsAllPagesInRange()
    {
        var result = PageRangeParser.Parse("2-5");
        Assert.Equal([2, 3, 4, 5], result);
    }

    [Fact]
    public void Parse_ReversedRange_NormalizesToAscending()
    {
        var result = PageRangeParser.Parse("5-2");
        Assert.Equal([2, 3, 4, 5], result);
    }

    [Fact]
    public void Parse_MixedRangesAndPages_ReturnsSortedDistinct()
    {
        var result = PageRangeParser.Parse("1-3, 5, 8-10");
        Assert.Equal([1, 2, 3, 5, 8, 9, 10], result);
    }

    [Fact]
    public void Parse_DuplicatePages_AreRemoved()
    {
        var result = PageRangeParser.Parse("1, 1, 2, 1-3");
        Assert.Equal([1, 2, 3], result);
    }

    [Fact]
    public void Parse_WithTotalPages_ClampsRangeEnd()
    {
        var result = PageRangeParser.Parse("1-100", totalPages: 5);
        Assert.Equal([1, 2, 3, 4, 5], result);
    }

    [Fact]
    public void Parse_WithTotalPages_FiltersSinglePagesAboveTotal()
    {
        var result = PageRangeParser.Parse("3, 10", totalPages: 5);
        Assert.Equal([3], result);
    }

    [Fact]
    public void Parse_ZeroPage_Ignored()
    {
        var result = PageRangeParser.Parse("0, 1, 2");
        Assert.Equal([1, 2], result);
    }

    [Fact]
    public void Parse_NegativeSinglePage_Ignored()
    {
        var result = PageRangeParser.Parse("-1, 2");
        Assert.Equal([2], result);
    }

    [Fact]
    public void Parse_RangeStartingAtZero_ClampedTo1()
    {
        var result = PageRangeParser.Parse("0-3");
        Assert.Equal([1, 2, 3], result);
    }

    [Fact]
    public void Parse_EmptyString_ReturnsEmptyList()
    {
        var result = PageRangeParser.Parse("");
        Assert.Empty(result);
    }

    [Fact]
    public void Parse_WhitespaceOnly_ReturnsEmptyList()
    {
        var result = PageRangeParser.Parse("   ");
        Assert.Empty(result);
    }

    [Fact]
    public void Parse_InvalidInput_ReturnsEmptyList()
    {
        var result = PageRangeParser.Parse("abc, xyz");
        Assert.Empty(result);
    }

    [Fact]
    public void Parse_WhitespaceAroundValues_IsTrimmed()
    {
        var result = PageRangeParser.Parse("  1 ,  3 - 5  ");
        Assert.Equal([1, 3, 4, 5], result);
    }

    [Fact]
    public void Parse_SinglePageWithTotalPages_ReturnsPage()
    {
        var result = PageRangeParser.Parse("3", totalPages: 10);
        Assert.Equal([3], result);
    }

    [Fact]
    public void Parse_MixedValidAndInvalid_ReturnsOnlyValid()
    {
        var result = PageRangeParser.Parse("1, abc, 3, , 5");
        Assert.Equal([1, 3, 5], result);
    }

    [Theory]
    [InlineData("1", 1)]
    [InlineData("1-5", 5)]
    [InlineData("1,2,3", 3)]
    [InlineData("1-3,5-7", 6)]
    public void Parse_VariousInputs_ReturnsExpectedCount(string input, int expectedCount)
    {
        var result = PageRangeParser.Parse(input);
        Assert.Equal(expectedCount, result.Count);
    }

    [Fact]
    public void Parse_ResultIsSorted()
    {
        var result = PageRangeParser.Parse("10, 1, 5, 3");
        Assert.Equal([1, 3, 5, 10], result);
    }
}
