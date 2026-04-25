using KeepPrinter.Core.Contracts;
using KeepPrinter.Core.Models;
using System.Text.Json;

namespace KeepPrinter.Infrastructure.Services;

/// <summary>
/// Implementación del almacenamiento de sesiones usando archivos JSON.
/// </summary>
public class SessionStore : ISessionStore
{
    private readonly string _storageFolder;
    private const string SessionFileName = "current_session.json";
    private const string SessionsFolder = "Sessions";

    public SessionStore()
    {
        // Usar ApplicationData local para Windows
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        _storageFolder = Path.Combine(appDataPath, "KeepPrinter", SessionsFolder);

        // Crear carpeta si no existe
        if (!Directory.Exists(_storageFolder))
        {
            Directory.CreateDirectory(_storageFolder);
        }
    }

    /// <summary>
    /// Constructor con ruta personalizada (útil para tests).
    /// </summary>
    public SessionStore(string customStoragePath)
    {
        _storageFolder = customStoragePath;

        if (!Directory.Exists(_storageFolder))
        {
            Directory.CreateDirectory(_storageFolder);
        }
    }

    /// <summary>
    /// Guarda una sesión en disco.
    /// </summary>
    public async Task SaveAsync(PrintSession session, CancellationToken cancellationToken = default)
    {
        if (session == null)
        {
            throw new ArgumentNullException(nameof(session));
        }

        var filePath = GetSessionFilePath(session.Id);

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(session, options);
        await File.WriteAllTextAsync(filePath, json, cancellationToken);

        // Guardar también como "current_session" para fácil recuperación
        var currentSessionPath = Path.Combine(_storageFolder, SessionFileName);
        await File.WriteAllTextAsync(currentSessionPath, json, cancellationToken);
    }

    /// <summary>
    /// Carga la última sesión guardada.
    /// </summary>
    public async Task<PrintSession?> LoadLastAsync(CancellationToken cancellationToken = default)
    {
        var currentSessionPath = Path.Combine(_storageFolder, SessionFileName);

        if (!File.Exists(currentSessionPath))
        {
            return null;
        }

        try
        {
            var json = await File.ReadAllTextAsync(currentSessionPath, cancellationToken);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return JsonSerializer.Deserialize<PrintSession>(json, options);
        }
        catch
        {
            // Si hay error al leer/deserializar, devolver null
            return null;
        }
    }

    /// <summary>
    /// Carga una sesión específica por su ID.
    /// </summary>
    public async Task<PrintSession?> LoadByIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var filePath = GetSessionFilePath(sessionId);

        if (!File.Exists(filePath))
        {
            return null;
        }

        try
        {
            var json = await File.ReadAllTextAsync(filePath, cancellationToken);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return JsonSerializer.Deserialize<PrintSession>(json, options);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Elimina una sesión guardada.
    /// </summary>
    public Task DeleteAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            var filePath = GetSessionFilePath(sessionId);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            // Si era la sesión actual, eliminar también ese archivo
            var currentSessionPath = Path.Combine(_storageFolder, SessionFileName);
            if (File.Exists(currentSessionPath))
            {
                // Leer y verificar si es la misma sesión
                try
                {
                    var json = File.ReadAllText(currentSessionPath);
                    var session = JsonSerializer.Deserialize<PrintSession>(json);
                    if (session?.Id == sessionId)
                    {
                        File.Delete(currentSessionPath);
                    }
                }
                catch
                {
                    // Ignorar errores
                }
            }
        }, cancellationToken);
    }

    /// <summary>
    /// Obtiene todas las sesiones guardadas.
    /// </summary>
    public async Task<List<PrintSession>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var sessions = new List<PrintSession>();

        try
        {
            var sessionFiles = Directory.GetFiles(_storageFolder, "session_*.json");

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            foreach (var filePath in sessionFiles)
            {
                try
                {
                    var json = await File.ReadAllTextAsync(filePath, cancellationToken);
                    var session = JsonSerializer.Deserialize<PrintSession>(json, options);
                    if (session != null)
                    {
                        sessions.Add(session);
                    }
                }
                catch
                {
                    // Ignorar archivos corruptos
                }
            }
        }
        catch
        {
            // Si hay error al leer el directorio, devolver lista vacía
        }

        return sessions;
    }

    /// <summary>
    /// Obtiene la ruta del archivo de sesión.
    /// </summary>
    private string GetSessionFilePath(Guid sessionId)
    {
        return Path.Combine(_storageFolder, $"session_{sessionId}.json");
    }
}
