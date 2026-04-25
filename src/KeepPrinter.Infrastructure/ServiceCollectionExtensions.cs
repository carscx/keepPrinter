using Microsoft.Extensions.DependencyInjection;
using KeepPrinter.Core.Contracts;
using KeepPrinter.Infrastructure.Services;

namespace KeepPrinter.Infrastructure;

/// <summary>
/// Extensiones para registrar servicios de infraestructura en el contenedor de DI.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra todos los servicios de infraestructura.
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Servicios singleton (una instancia para toda la app)
        services.AddSingleton<IPdfService, PdfService>();
        services.AddSingleton<IPrintService, PrintService>();
        services.AddSingleton<ISessionStore, SessionStore>();

        return services;
    }
}
