using KeepPrinter.Core.Models;
using KeepPrinter.Core.Services;

namespace KeepPrinter.Core.Tests.Services;

public class BatchCalculatorTests
{
    private readonly BatchCalculator _calculator = new();

    [Fact]
    public void CalculateBatches_SimpleCase_ReturnsCorrectBatches()
    {
        var session = new PrintSession
        {
            TotalPages = 100,
            StartPage = 1,
            SheetsPerBatch = 50
        };

        var batches = _calculator.CalculateBatches(session);

        Assert.Equal(1, batches.Count);
        Assert.Equal(0, batches[0].Index);
        Assert.Equal(1, batches[0].StartPage);
        Assert.Equal(100, batches[0].EndPage);
        Assert.Equal(50, batches[0].SheetCount);
    }

    [Fact]
    public void CalculateBatches_MultipleBatches_ReturnsCorrectDistribution()
    {
        var session = new PrintSession
        {
            TotalPages = 100,
            StartPage = 1,
            SheetsPerBatch = 25
        };

        var batches = _calculator.CalculateBatches(session);

        Assert.Equal(2, batches.Count);

        Assert.Equal(0, batches[0].Index);
        Assert.Equal(1, batches[0].StartPage);
        Assert.Equal(50, batches[0].EndPage);
        Assert.Equal(25, batches[0].SheetCount);

        Assert.Equal(1, batches[1].Index);
        Assert.Equal(51, batches[1].StartPage);
        Assert.Equal(100, batches[1].EndPage);
        Assert.Equal(25, batches[1].SheetCount);
    }

    [Fact]
    public void CalculateBatches_OddPages_HandlesCorrectly()
    {
        var session = new PrintSession
        {
            TotalPages = 101,
            StartPage = 1,
            SheetsPerBatch = 25
        };

        var batches = _calculator.CalculateBatches(session);

        Assert.Equal(3, batches.Count);

        Assert.Equal(101, batches[2].StartPage);
        Assert.Equal(101, batches[2].EndPage);
        Assert.Equal(1, batches[2].SheetCount); // 1 página = 1 hoja
    }

    [Fact]
    public void CalculateBatches_StartFromMiddle_ReturnsCorrectBatches()
    {
        var session = new PrintSession
        {
            TotalPages = 100,
            StartPage = 51,
            SheetsPerBatch = 25
        };

        var batches = _calculator.CalculateBatches(session);

        Assert.Equal(1, batches.Count);
        Assert.Equal(51, batches[0].StartPage);
        Assert.Equal(100, batches[0].EndPage);
        Assert.Equal(25, batches[0].SheetCount);
    }

    [Fact]
    public void GetOddPages_ReturnsOnlyOddPages()
    {
        var pages = _calculator.GetOddPages(1, 10);

        Assert.Equal(new[] { 1, 3, 5, 7, 9 }, pages);
    }

    [Fact]
    public void GetOddPages_StartFromEven_ReturnsCorrectPages()
    {
        var pages = _calculator.GetOddPages(2, 10);

        Assert.Equal(new[] { 3, 5, 7, 9 }, pages);
    }

    [Fact]
    public void GetEvenPages_ReturnsOnlyEvenPages()
    {
        var pages = _calculator.GetEvenPages(1, 10);

        Assert.Equal(new[] { 2, 4, 6, 8, 10 }, pages);
    }

    [Fact]
    public void GetEvenPages_NoEvenPages_ReturnsEmpty()
    {
        var pages = _calculator.GetEvenPages(1, 1);

        Assert.Empty(pages);
    }

    [Fact]
    public void GetOddPages_SingleOddPage_ReturnsSinglePage()
    {
        var pages = _calculator.GetOddPages(5, 5);

        Assert.Single(pages);
        Assert.Equal(5, pages[0]);
    }

    [Fact]
    public void GetEvenPages_SingleEvenPage_ReturnsSinglePage()
    {
        var pages = _calculator.GetEvenPages(6, 6);

        Assert.Single(pages);
        Assert.Equal(6, pages[0]);
    }

    [Fact]
    public void CalculateBatches_VerySmallBatch_Works()
    {
        var session = new PrintSession
        {
            TotalPages = 10,
            StartPage = 1,
            SheetsPerBatch = 2
        };

        var batches = _calculator.CalculateBatches(session);

        Assert.Equal(3, batches.Count);
        Assert.Equal(1, batches[0].StartPage);
        Assert.Equal(4, batches[0].EndPage);
        Assert.Equal(5, batches[1].StartPage);
        Assert.Equal(8, batches[1].EndPage);
        Assert.Equal(9, batches[2].StartPage);
        Assert.Equal(10, batches[2].EndPage);
    }
}
