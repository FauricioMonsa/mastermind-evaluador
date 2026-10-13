using System.Globalization;

namespace Mastermind.Core.Tests;

public class EvaluadorLoggingTests
{
    [Fact]
    public void EvaluarIntento_EvaluacionCorrecta_RegistraMensajeConFechaControlada()
    {
        var registro = new RegistroMemoria();
        var tiempo = new ProveedorTiempoFijo(
            new DateTimeOffset(2025, 3, 4, 5, 6, 7, TimeSpan.Zero));
        var evaluador = new Evaluador(logEvaluaciones: registro, timeProvider: tiempo);

        var resultado = evaluador.EvaluarIntento("1234", "4321");

        Assert.Equal(new ResultadoEvaluacion(0, 4), resultado);
        Assert.Equal(
            "En la fecha 04-03-2025 05:06:07 se evaluó el intento 4321 contra el patrón 1234. Resultado: 0 bien posicionados – 4 con color correcto.",
            Assert.Single(registro.Mensajes));
    }

    [Fact]
    public void EvaluarIntento_EntradaInvalida_NoRegistraMensaje()
    {
        var registro = new RegistroMemoria();
        var evaluador = new Evaluador(logEvaluaciones: registro);

        Assert.Throws<ArgumentException>(() => evaluador.EvaluarIntento("123", "123"));

        Assert.Empty(registro.Mensajes);
    }

    [Fact]
    public void LogEvaluacionesConsola_Registrar_EscribeEnConsolaYArchivoOpcional()
    {
        var mensaje = "Línea de prueba.";
        var rutaArchivo = Path.Combine(AppContext.BaseDirectory, $"{Guid.NewGuid():N}.log");
        var salidaOriginal = Console.Out;
        using var salida = new StringWriter(CultureInfo.InvariantCulture);

        try
        {
            Console.SetOut(salida);
            new LogEvaluacionesConsola(rutaArchivo).Registrar(mensaje);

            Assert.Equal(mensaje + Environment.NewLine, salida.ToString());
            Assert.Equal(mensaje + Environment.NewLine, File.ReadAllText(rutaArchivo));
        }
        finally
        {
            Console.SetOut(salidaOriginal);
            if (File.Exists(rutaArchivo))
            {
                File.Delete(rutaArchivo);
            }
        }
    }

    private sealed class RegistroMemoria : ILogEvaluaciones
    {
        public List<string> Mensajes { get; } = [];

        public void Registrar(string mensaje) => Mensajes.Add(mensaje);
    }

    private sealed class ProveedorTiempoFijo : TimeProvider
    {
        private readonly DateTimeOffset _instante;

        public ProveedorTiempoFijo(DateTimeOffset instante)
        {
            _instante = instante;
        }

        public override DateTimeOffset GetUtcNow() => _instante;

        public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
    }
}