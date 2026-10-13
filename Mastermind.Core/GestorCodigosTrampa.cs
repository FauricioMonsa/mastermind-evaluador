using System.Text.Json;

namespace Mastermind.Core;

public sealed class GestorCodigosTrampa
{
    private static readonly JsonSerializerOptions OpcionesJson = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    private readonly string _rutaArchivo;

    public GestorCodigosTrampa(string? rutaArchivo = null)
    {
        _rutaArchivo = string.IsNullOrWhiteSpace(rutaArchivo)
            ? Path.Combine(AppContext.BaseDirectory, "cheatcodes.json")
            : rutaArchivo;
    }

    public int ConsultarUsosDisponibles(string hash)
    {
        var codigo = CargarCodigos().FirstOrDefault(codigo =>
            string.Equals(codigo.Hash, hash, StringComparison.Ordinal));

        return codigo?.UsosDisponibles ?? 0;
    }

    public bool IntentarConsumirUso(string hash)
    {
        var codigos = CargarCodigos();
        var codigo = codigos.FirstOrDefault(codigo =>
            string.Equals(codigo.Hash, hash, StringComparison.Ordinal));

        if (codigo is null || codigo.UsosDisponibles <= 0)
        {
            return false;
        }

        codigo.UsosDisponibles--;
        File.WriteAllText(_rutaArchivo, JsonSerializer.Serialize(codigos, OpcionesJson));
        return true;
    }

    private List<EntradaCodigoTrampa> CargarCodigos()
    {
        if (!File.Exists(_rutaArchivo))
        {
            return [];
        }

        return JsonSerializer.Deserialize<List<EntradaCodigoTrampa>>(
            File.ReadAllText(_rutaArchivo), OpcionesJson) ?? [];
    }

    private sealed class EntradaCodigoTrampa
    {
        public EntradaCodigoTrampa()
        {
        }

        public string Hash { get; set; } = string.Empty;

        public int UsosDisponibles { get; set; }
    }
}