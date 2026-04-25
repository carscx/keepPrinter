using KeepPrinter.Core.Models;
using KeepPrinter.Infrastructure.Services;
using KeepPrinter.Infrastructure.Tests.Helpers;

namespace KeepPrinter.Infrastructure.Tests.Services;

public class PdfServiceTests : IDisposable
{
    private readonly PdfService _pdfService;
    private readonly List<string> _tempFiles;
    private readonly string _testOutputFolder;

    public PdfServiceTests()
    {
        _pdfService = new PdfService();
        _tempFiles = new List<string>();
        _testOutputFolder = Path.Combine(Path.GetTempPath(), $"KeepPrinterTests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testOutputFolder);
    }

    public void Dispose()
    {
        // Limpiar archivos temporales
        foreach (var file in _tempFiles)
        {
            if (File.Exists(file))
            {
                try { File.Delete(file); } catch { }
            }
        }

        // Limpiar carpeta de salida de tests
        if (Directory.Exists(_testOutputFolder))
        {
            try { Directory.Delete(_testOutputFolder, recursive: true); } catch { }
        }
    }

    private string RegisterTempFile(string path)
    {
        _tempFiles.Add(path);
        return path;
    }

    #region GetPageCountAsync Tests

    [Fact]
    public async Task GetPageCountAsync_ValidPdf_ReturnsCorrectCount()
    {
        // Arrange
        var testPdf = RegisterTempFile(TestPdfGenerator.CreateTemporaryTestPdf(10));

        // Act
        var pageCount = await _pdfService.GetPageCountAsync(testPdf);

        // Assert
        Assert.Equal(10, pageCount);
    }

    [Fact]
    public async Task GetPageCountAsync_SinglePagePdf_ReturnsOne()
    {
        // Arrange
        var testPdf = RegisterTempFile(TestPdfGenerator.CreateTemporaryTestPdf(1));

        // Act
        var pageCount = await _pdfService.GetPageCountAsync(testPdf);

        // Assert
        Assert.Equal(1, pageCount);
    }

    [Fact]
    public async Task GetPageCountAsync_NonExistentFile_ThrowsFileNotFoundException()
    {
        // Arrange
        var nonExistentPath = Path.Combine(Path.GetTempPath(), "nonexistent.pdf");

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(
            () => _pdfService.GetPageCountAsync(nonExistentPath));
    }

    #endregion

    #region ExtractPagesAsync Tests

    [Fact]
    public async Task ExtractPagesAsync_ExtractSpecificPages_CreatesCorrectPdf()
    {
        // Arrange
        var sourcePdf = RegisterTempFile(TestPdfGenerator.CreateTemporaryTestPdf(10));
        var outputPdf = RegisterTempFile(Path.Combine(_testOutputFolder, "extracted.pdf"));
        var pagesToExtract = new List<int> { 1, 3, 5, 7, 9 };

        // Act
        await _pdfService.ExtractPagesAsync(sourcePdf, outputPdf, pagesToExtract);

        // Assert
        Assert.True(File.Exists(outputPdf));
        var extractedPageCount = await _pdfService.GetPageCountAsync(outputPdf);
        Assert.Equal(5, extractedPageCount);
    }

    [Fact]
    public async Task ExtractPagesAsync_ExtractSinglePage_CreatesOnePagePdf()
    {
        // Arrange
        var sourcePdf = RegisterTempFile(TestPdfGenerator.CreateTemporaryTestPdf(10));
        var outputPdf = RegisterTempFile(Path.Combine(_testOutputFolder, "single.pdf"));
        var pagesToExtract = new List<int> { 5 };

        // Act
        await _pdfService.ExtractPagesAsync(sourcePdf, outputPdf, pagesToExtract);

        // Assert
        Assert.True(File.Exists(outputPdf));
        var extractedPageCount = await _pdfService.GetPageCountAsync(outputPdf);
        Assert.Equal(1, extractedPageCount);
    }

    [Fact]
    public async Task ExtractPagesAsync_ExtractAllPages_CreatesIdenticalPdf()
    {
        // Arrange
        var sourcePdf = RegisterTempFile(TestPdfGenerator.CreateTemporaryTestPdf(5));
        var outputPdf = RegisterTempFile(Path.Combine(_testOutputFolder, "all_pages.pdf"));
        var pagesToExtract = new List<int> { 1, 2, 3, 4, 5 };

        // Act
        await _pdfService.ExtractPagesAsync(sourcePdf, outputPdf, pagesToExtract);

        // Assert
        var extractedPageCount = await _pdfService.GetPageCountAsync(outputPdf);
        Assert.Equal(5, extractedPageCount);
    }

    [Fact]
    public async Task ExtractPagesAsync_EmptyPageList_ThrowsArgumentException()
    {
        // Arrange
        var sourcePdf = RegisterTempFile(TestPdfGenerator.CreateTemporaryTestPdf(10));
        var outputPdf = Path.Combine(_testOutputFolder, "output.pdf");
        var emptyList = new List<int>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _pdfService.ExtractPagesAsync(sourcePdf, outputPdf, emptyList));
    }

    [Fact]
    public async Task ExtractPagesAsync_PageOutOfRange_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var sourcePdf = RegisterTempFile(TestPdfGenerator.CreateTemporaryTestPdf(10));
        var outputPdf = Path.Combine(_testOutputFolder, "output.pdf");
        var invalidPages = new List<int> { 1, 15 }; // 15 está fuera de rango

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => _pdfService.ExtractPagesAsync(sourcePdf, outputPdf, invalidPages));
    }

    [Fact]
    public async Task ExtractPagesAsync_NonExistentSourceFile_ThrowsFileNotFoundException()
    {
        // Arrange
        var nonExistentPdf = "nonexistent.pdf";
        var outputPdf = Path.Combine(_testOutputFolder, "output.pdf");
        var pages = new List<int> { 1 };

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(
            () => _pdfService.ExtractPagesAsync(nonExistentPdf, outputPdf, pages));
    }

    #endregion

    #region GenerateBatchPdfsAsync Tests

    [Fact]
    public async Task GenerateBatchPdfsAsync_FirstBatch_GeneratesCorrectFrontAndBackPdfs()
    {
        // Arrange
        var sourcePdf = RegisterTempFile(TestPdfGenerator.CreateTemporaryTestPdf(100));

        var session = new PrintSession
        {
            SourcePdfPath = sourcePdf,
            OutputFolder = _testOutputFolder,
            TotalPages = 100,
            StartPage = 1,
            SheetsPerBatch = 50
        };

        var batch = new TandaInfo
        {
            Index = 0,
            StartPage = 1,
            EndPage = 100,
            SheetCount = 50
        };

        // Act
        var (frontPath, backPath) = await _pdfService.GenerateBatchPdfsAsync(session, batch);
        RegisterTempFile(frontPath);
        RegisterTempFile(backPath);

        // Assert
        Assert.True(File.Exists(frontPath));
        Assert.True(File.Exists(backPath));
        Assert.Contains("batch_000_front.pdf", frontPath);
        Assert.Contains("batch_000_back.pdf", backPath);

        // Verificar que el frente tiene páginas impares (1, 3, 5, ..., 99)
        var frontPageCount = await _pdfService.GetPageCountAsync(frontPath);
        Assert.Equal(50, frontPageCount);

        // Verificar que el dorso tiene páginas pares (2, 4, 6, ..., 100)
        var backPageCount = await _pdfService.GetPageCountAsync(backPath);
        Assert.Equal(50, backPageCount);
    }

    [Fact]
    public async Task GenerateBatchPdfsAsync_SecondBatch_GeneratesCorrectlyNumberedFiles()
    {
        // Arrange
        var sourcePdf = RegisterTempFile(TestPdfGenerator.CreateTemporaryTestPdf(200));

        var session = new PrintSession
        {
            SourcePdfPath = sourcePdf,
            OutputFolder = _testOutputFolder,
            TotalPages = 200,
            StartPage = 1,
            SheetsPerBatch = 50
        };

        var batch = new TandaInfo
        {
            Index = 1, // Segunda tanda
            StartPage = 101,
            EndPage = 200,
            SheetCount = 50
        };

        // Act
        var (frontPath, backPath) = await _pdfService.GenerateBatchPdfsAsync(session, batch);
        RegisterTempFile(frontPath);
        RegisterTempFile(backPath);

        // Assert
        Assert.Contains("batch_001_front.pdf", frontPath);
        Assert.Contains("batch_001_back.pdf", backPath);
    }

    [Fact]
    public async Task GenerateBatchPdfsAsync_OddNumberOfPages_GeneratesCorrectPdfs()
    {
        // Arrange: PDF de 99 páginas (impar)
        var sourcePdf = RegisterTempFile(TestPdfGenerator.CreateTemporaryTestPdf(99));

        var session = new PrintSession
        {
            SourcePdfPath = sourcePdf,
            OutputFolder = _testOutputFolder,
            TotalPages = 99,
            StartPage = 1,
            SheetsPerBatch = 50
        };

        var batch = new TandaInfo
        {
            Index = 0,
            StartPage = 1,
            EndPage = 99,
            SheetCount = 50 // 49.5 hojas realmente, pero contamos 50
        };

        // Act
        var (frontPath, backPath) = await _pdfService.GenerateBatchPdfsAsync(session, batch);
        RegisterTempFile(frontPath);
        RegisterTempFile(backPath);

        // Assert
        var frontPageCount = await _pdfService.GetPageCountAsync(frontPath);
        var backPageCount = await _pdfService.GetPageCountAsync(backPath);

        // Frente: 1, 3, 5, ..., 99 = 50 páginas
        Assert.Equal(50, frontPageCount);

        // Dorso: 2, 4, 6, ..., 98 = 49 páginas (no hay página 100)
        Assert.Equal(49, backPageCount);
    }

    [Fact]
    public async Task GenerateBatchPdfsAsync_SmallBatch_GeneratesCorrectPdfs()
    {
        // Arrange: Tanda de solo 4 hojas (8 páginas)
        var sourcePdf = RegisterTempFile(TestPdfGenerator.CreateTemporaryTestPdf(20));

        var session = new PrintSession
        {
            SourcePdfPath = sourcePdf,
            OutputFolder = _testOutputFolder,
            TotalPages = 20,
            StartPage = 1,
            SheetsPerBatch = 4
        };

        var batch = new TandaInfo
        {
            Index = 0,
            StartPage = 1,
            EndPage = 8,
            SheetCount = 4
        };

        // Act
        var (frontPath, backPath) = await _pdfService.GenerateBatchPdfsAsync(session, batch);
        RegisterTempFile(frontPath);
        RegisterTempFile(backPath);

        // Assert
        var frontPageCount = await _pdfService.GetPageCountAsync(frontPath);
        var backPageCount = await _pdfService.GetPageCountAsync(backPath);

        // Frente: 1, 3, 5, 7 = 4 páginas
        Assert.Equal(4, frontPageCount);

        // Dorso: 2, 4, 6, 8 = 4 páginas
        Assert.Equal(4, backPageCount);
    }

    [Fact]
    public async Task GenerateBatchPdfsAsync_CreatesOutputFolderIfNotExists()
    {
        // Arrange
        var sourcePdf = RegisterTempFile(TestPdfGenerator.CreateTemporaryTestPdf(10));
        var nonExistentFolder = Path.Combine(_testOutputFolder, "subfolder", "nested");

        var session = new PrintSession
        {
            SourcePdfPath = sourcePdf,
            OutputFolder = nonExistentFolder,
            TotalPages = 10,
            StartPage = 1,
            SheetsPerBatch = 5
        };

        var batch = new TandaInfo
        {
            Index = 0,
            StartPage = 1,
            EndPage = 10,
            SheetCount = 5
        };

        // Act
        var (frontPath, backPath) = await _pdfService.GenerateBatchPdfsAsync(session, batch);
        RegisterTempFile(frontPath);
        RegisterTempFile(backPath);

        // Assert
        Assert.True(Directory.Exists(nonExistentFolder));
        Assert.True(File.Exists(frontPath));
        Assert.True(File.Exists(backPath));
    }

    #endregion
}
