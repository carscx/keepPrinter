using KeepPrinter.Core.Models;

namespace KeepPrinter.Core.Tests.Models;

public class PrintSessionTests
{
    [Fact]
    public void Constructor_SetsDefaultValues()
    {
        var session = new PrintSession();

        Assert.NotEqual(Guid.Empty, session.Id);
        Assert.Equal(string.Empty, session.SourcePdfPath);
        Assert.Equal(string.Empty, session.OutputFolder);
        Assert.Equal(1, session.StartPage);
        Assert.Equal(50, session.SheetsPerBatch);
        Assert.Equal(0, session.CurrentBatchIndex);
        Assert.Equal(WorkflowStage.NotPrepared, session.CurrentStage);
        Assert.NotNull(session.Batches);
        Assert.Empty(session.Batches);
    }

    [Fact]
    public void CurrentBatch_NoBatches_ReturnsNull()
    {
        var session = new PrintSession
        {
            CurrentBatchIndex = 0,
            Batches = new List<TandaInfo>()
        };

        Assert.Null(session.CurrentBatch);
    }

    [Fact]
    public void CurrentBatch_WithBatches_ReturnsCorrectBatch()
    {
        var batch1 = new TandaInfo { Index = 0 };
        var batch2 = new TandaInfo { Index = 1 };

        var session = new PrintSession
        {
            CurrentBatchIndex = 1,
            Batches = new List<TandaInfo> { batch1, batch2 }
        };

        Assert.Equal(batch2, session.CurrentBatch);
    }

    [Fact]
    public void CurrentBatch_InvalidIndex_ReturnsNull()
    {
        var session = new PrintSession
        {
            CurrentBatchIndex = 5,
            Batches = new List<TandaInfo> { new TandaInfo { Index = 0 } }
        };

        Assert.Null(session.CurrentBatch);
    }

    [Fact]
    public void HasMoreBatches_AtLastBatch_ReturnsFalse()
    {
        var session = new PrintSession
        {
            CurrentBatchIndex = 2,
            Batches = new List<TandaInfo>
            {
                new TandaInfo(),
                new TandaInfo(),
                new TandaInfo()
            }
        };

        Assert.False(session.HasMoreBatches);
    }

    [Fact]
    public void HasMoreBatches_NotAtLastBatch_ReturnsTrue()
    {
        var session = new PrintSession
        {
            CurrentBatchIndex = 0,
            Batches = new List<TandaInfo>
            {
                new TandaInfo(),
                new TandaInfo()
            }
        };

        Assert.True(session.HasMoreBatches);
    }

    [Fact]
    public void IsFinished_StageFinished_ReturnsTrue()
    {
        var session = new PrintSession
        {
            CurrentStage = WorkflowStage.Finished
        };

        Assert.True(session.IsFinished);
    }

    [Fact]
    public void IsFinished_OtherStages_ReturnsFalse()
    {
        var stages = new[]
        {
            WorkflowStage.NotPrepared,
            WorkflowStage.Prepared,
            WorkflowStage.PendingFront,
            WorkflowStage.PendingBack,
            WorkflowStage.BatchComplete
        };

        foreach (var stage in stages)
        {
            var session = new PrintSession { CurrentStage = stage };
            Assert.False(session.IsFinished);
        }
    }
}
