using KeepPrinter.Core.Exceptions;
using KeepPrinter.Core.Models;
using KeepPrinter.Core.Validation;

namespace KeepPrinter.Core.Tests.Validation;

public class PrintSessionValidatorTests
{
    [Fact]
    public void ValidateInitialParameters_ValidSession_DoesNotThrow()
    {
        var session = new PrintSession
        {
            SourcePdfPath = @"C:\test.pdf",
            TotalPages = 100,
            StartPage = 1,
            SheetsPerBatch = 50,
            OutputFolder = @"C:\output"
        };

        var exception = Record.Exception(() => PrintSessionValidator.ValidateInitialParameters(session));

        Assert.Null(exception);
    }

    [Fact]
    public void ValidateInitialParameters_EmptySourcePath_ThrowsValidationException()
    {
        var session = new PrintSession
        {
            SourcePdfPath = "",
            TotalPages = 100,
            OutputFolder = @"C:\output"
        };

        var exception = Assert.Throws<ValidationException>(
            () => PrintSessionValidator.ValidateInitialParameters(session));

        Assert.Contains("ruta del archivo PDF", exception.Message);
    }

    [Fact]
    public void ValidateInitialParameters_ZeroTotalPages_ThrowsValidationException()
    {
        var session = new PrintSession
        {
            SourcePdfPath = @"C:\test.pdf",
            TotalPages = 0,
            OutputFolder = @"C:\output"
        };

        var exception = Assert.Throws<ValidationException>(
            () => PrintSessionValidator.ValidateInitialParameters(session));

        Assert.Contains("páginas debe ser mayor que cero", exception.Message);
    }

    [Fact]
    public void ValidateInitialParameters_StartPageLessThanOne_ThrowsValidationException()
    {
        var session = new PrintSession
        {
            SourcePdfPath = @"C:\test.pdf",
            TotalPages = 100,
            StartPage = 0,
            OutputFolder = @"C:\output"
        };

        var exception = Assert.Throws<ValidationException>(
            () => PrintSessionValidator.ValidateInitialParameters(session));

        Assert.Contains("página inicial debe ser mayor o igual a 1", exception.Message);
    }

    [Fact]
    public void ValidateInitialParameters_StartPageGreaterThanTotal_ThrowsValidationException()
    {
        var session = new PrintSession
        {
            SourcePdfPath = @"C:\test.pdf",
            TotalPages = 100,
            StartPage = 150,
            OutputFolder = @"C:\output"
        };

        var exception = Assert.Throws<ValidationException>(
            () => PrintSessionValidator.ValidateInitialParameters(session));

        Assert.Contains("no puede ser mayor que el total", exception.Message);
    }

    [Fact]
    public void ValidateInitialParameters_ZeroSheetsPerBatch_ThrowsValidationException()
    {
        var session = new PrintSession
        {
            SourcePdfPath = @"C:\test.pdf",
            TotalPages = 100,
            SheetsPerBatch = 0,
            OutputFolder = @"C:\output"
        };

        var exception = Assert.Throws<ValidationException>(
            () => PrintSessionValidator.ValidateInitialParameters(session));

        Assert.Contains("hojas por tanda debe ser mayor que cero", exception.Message);
    }

    [Fact]
    public void ValidateInitialParameters_TooManySheetsPerBatch_ThrowsValidationException()
    {
        var session = new PrintSession
        {
            SourcePdfPath = @"C:\test.pdf",
            TotalPages = 100,
            SheetsPerBatch = 1500,
            OutputFolder = @"C:\output"
        };

        var exception = Assert.Throws<ValidationException>(
            () => PrintSessionValidator.ValidateInitialParameters(session));

        Assert.Contains("no puede exceder 1000", exception.Message);
    }

    [Fact]
    public void ValidateReadyForFrontPrint_ValidTanda_DoesNotThrow()
    {
        var tanda = new TandaInfo
        {
            FrontPdfPath = @"C:\output\front.pdf",
            FrontPrinted = false
        };

        var exception = Record.Exception(() => PrintSessionValidator.ValidateReadyForFrontPrint(tanda));

        Assert.Null(exception);
    }

    [Fact]
    public void ValidateReadyForFrontPrint_NoFrontPdf_ThrowsValidationException()
    {
        var tanda = new TandaInfo
        {
            FrontPdfPath = null,
            FrontPrinted = false
        };

        var exception = Assert.Throws<ValidationException>(
            () => PrintSessionValidator.ValidateReadyForFrontPrint(tanda));

        Assert.Contains("no tiene un archivo PDF de frente", exception.Message);
    }

    [Fact]
    public void ValidateReadyForFrontPrint_AlreadyPrinted_ThrowsValidationException()
    {
        var tanda = new TandaInfo
        {
            FrontPdfPath = @"C:\output\front.pdf",
            FrontPrinted = true
        };

        var exception = Assert.Throws<ValidationException>(
            () => PrintSessionValidator.ValidateReadyForFrontPrint(tanda));

        Assert.Contains("ya ha sido impreso", exception.Message);
    }

    [Fact]
    public void ValidateReadyForBackPrint_ValidTanda_DoesNotThrow()
    {
        var tanda = new TandaInfo
        {
            BackPdfPath = @"C:\output\back.pdf",
            FrontPrinted = true,
            BackPrinted = false
        };

        var exception = Record.Exception(() => PrintSessionValidator.ValidateReadyForBackPrint(tanda));

        Assert.Null(exception);
    }

    [Fact]
    public void ValidateReadyForBackPrint_FrontNotPrinted_ThrowsValidationException()
    {
        var tanda = new TandaInfo
        {
            BackPdfPath = @"C:\output\back.pdf",
            FrontPrinted = false,
            BackPrinted = false
        };

        var exception = Assert.Throws<ValidationException>(
            () => PrintSessionValidator.ValidateReadyForBackPrint(tanda));

        Assert.Contains("Debe imprimir el frente antes", exception.Message);
    }

    [Fact]
    public void ValidateCanConfirmBatch_ValidTanda_DoesNotThrow()
    {
        var tanda = new TandaInfo
        {
            FrontPrinted = true,
            BackPrinted = true,
            IsComplete = false
        };

        var exception = Record.Exception(() => PrintSessionValidator.ValidateCanConfirmBatch(tanda));

        Assert.Null(exception);
    }

    [Fact]
    public void ValidateCanConfirmBatch_NotFullyPrinted_ThrowsValidationException()
    {
        var tanda = new TandaInfo
        {
            FrontPrinted = true,
            BackPrinted = false,
            IsComplete = false
        };

        var exception = Assert.Throws<ValidationException>(
            () => PrintSessionValidator.ValidateCanConfirmBatch(tanda));

        Assert.Contains("sin haber impreso el dorso", exception.Message);
    }

    [Fact]
    public void ValidateCanAdvanceBatch_ValidSession_DoesNotThrow()
    {
        var session = new PrintSession
        {
            CurrentBatchIndex = 0,
            Batches = new List<TandaInfo>
            {
                new TandaInfo { IsComplete = true },
                new TandaInfo { IsComplete = false }
            }
        };

        var exception = Record.Exception(() => PrintSessionValidator.ValidateCanAdvanceBatch(session));

        Assert.Null(exception);
    }

    [Fact]
    public void ValidateCanAdvanceBatch_CurrentNotComplete_ThrowsValidationException()
    {
        var session = new PrintSession
        {
            CurrentBatchIndex = 0,
            Batches = new List<TandaInfo>
            {
                new TandaInfo { IsComplete = false },
                new TandaInfo { IsComplete = false }
            }
        };

        var exception = Assert.Throws<ValidationException>(
            () => PrintSessionValidator.ValidateCanAdvanceBatch(session));

        Assert.Contains("confirmar la tanda actual", exception.Message);
    }

    [Fact]
    public void ValidateCanAdvanceBatch_NoMoreBatches_ThrowsValidationException()
    {
        var session = new PrintSession
        {
            CurrentBatchIndex = 1,
            Batches = new List<TandaInfo>
            {
                new TandaInfo { IsComplete = true },
                new TandaInfo { IsComplete = true }
            }
        };

        var exception = Assert.Throws<ValidationException>(
            () => PrintSessionValidator.ValidateCanAdvanceBatch(session));

        Assert.Contains("No hay más tandas", exception.Message);
    }
}
