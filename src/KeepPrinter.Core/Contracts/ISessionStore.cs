using KeepPrinter.Core.Models;

namespace KeepPrinter.Core.Contracts;

/// <summary>
/// Contrato para persistencia y recuperación de sesiones de impresión.
/// </summary>
public interface ISessionStore
{
    /// <summary>
    /// Guarda una sesión en el almacenamiento persistente.
    /// </summary>
    /// <param name="session">Sesión a guardar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    Task SaveAsync(PrintSession session, CancellationToken cancellationToken = default);

    /// <summary>
    /// Carga la última sesión guardada.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>La sesión cargada o null si no existe.</returns>
    Task<PrintSession?> LoadLastAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Carga una sesión específica por su ID.
    /// </summary>
    /// <param name="sessionId">ID de la sesión.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>La sesión cargada o null si no existe.</returns>
    Task<PrintSession?> LoadByIdAsync(Guid sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina una sesión del almacenamiento.
    /// </summary>
    /// <param name="sessionId">ID de la sesión a eliminar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    Task DeleteAsync(Guid sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todas las sesiones guardadas.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Lista de sesiones.</returns>
    Task<List<PrintSession>> GetAllAsync(CancellationToken cancellationToken = default);
}
