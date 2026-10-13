namespace Mastermind.Core;

public sealed class LogEvaluacionesConsola : ILogEvaluaciones
{
    private readonly string? _rutaArchivo;

    public LogEvaluacionesConsola(string? rutaArchivo = null)
    {
        _rutaArchivo = string.IsNullOrWhiteSpace(rutaArchivo) ? null : rutaArchivo;
    }

    public void Registrar(string mensaje)
    {
        Console.WriteLine(mensaje);

        if (_rutaArchivo is not null)
        {
            File.AppendAllText(_rutaArchivo, mensaje + Environment.NewLine);
        }
    }
}