using KeepPrinter.Core.Models;

namespace KeepPrinter.Core.Services;

/// <summary>
/// Servicio de dominio para calcular la distribución de páginas en tandas.
/// </summary>
public class BatchCalculator
{
    /// <summary>
    /// Calcula la lista de tandas basándose en los parámetros de la sesión.
    /// </summary>
    /// <param name="session">Sesión con los parámetros configurados.</param>
    /// <returns>Lista de TandaInfo con las páginas distribuidas.</returns>
    public List<TandaInfo> CalculateBatches(PrintSession session)
    {
        var batches = new List<TandaInfo>();
        int currentPage = session.StartPage;
        int batchIndex = 0;

        // Calculamos cuántas páginas lógicas quedan por procesar
        int remainingPages = session.TotalPages - session.StartPage + 1;

        while (currentPage <= session.TotalPages)
        {
            // Cada "hoja" tiene 2 páginas (frente y dorso)
            int pagesInThisBatch = Math.Min(session.SheetsPerBatch * 2, session.TotalPages - currentPage + 1);
            int sheetsInThisBatch = (int)Math.Ceiling(pagesInThisBatch / 2.0);

            var tanda = new TandaInfo
            {
                Index = batchIndex,
                StartPage = currentPage,
                EndPage = currentPage + pagesInThisBatch - 1,
                SheetCount = sheetsInThisBatch
            };

            batches.Add(tanda);

            currentPage += pagesInThisBatch;
            batchIndex++;
        }

        return batches;
    }

    /// <summary>
    /// Obtiene las páginas impares (frentes) de un rango.
    /// </summary>
    /// <param name="startPage">Página inicial (base 1).</param>
    /// <param name="endPage">Página final (base 1).</param>
    /// <returns>Lista de números de página impares.</returns>
    public List<int> GetOddPages(int startPage, int endPage)
    {
        var pages = new List<int>();

        for (int page = startPage; page <= endPage; page++)
        {
            if (page % 2 == 1) // Impares (frente)
            {
                pages.Add(page);
            }
        }

        return pages;
    }

    /// <summary>
    /// Obtiene las páginas pares (dorsos) de un rango.
    /// </summary>
    /// <param name="startPage">Página inicial (base 1).</param>
    /// <param name="endPage">Página final (base 1).</param>
    /// <returns>Lista de números de página pares.</returns>
    public List<int> GetEvenPages(int startPage, int endPage)
    {
        var pages = new List<int>();

        for (int page = startPage; page <= endPage; page++)
        {
            if (page % 2 == 0) // Pares (dorso)
            {
                pages.Add(page);
            }
        }

        return pages;
    }
}
