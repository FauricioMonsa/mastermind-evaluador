using System.Text.Json;

namespace Mastermind.Core.Tests;

public class EvaluadorCheatTests
{
    [Fact]
    public void EvaluarIntento_CodigoValido_RevelaColorEnPosicionAleatoria()
    {
        var ruta = CrearArchivo("""
            [{ "hash": "codigo-valido", "usosDisponibles": 2 }]
            """);

        try
        {
            var gestor = new GestorCodigosTrampa(ruta);
            var evaluador = CrearEvaluador(gestor, new RandomFijo(1));

            var resultado = evaluador.EvaluarIntento("1234", "1424", "codigo-valido");

            Assert.Equal(new ResultadoEvaluacion(2, 1, true, 2, '2'), resultado);
            Assert.Equal(2, resultado.PosicionRevelada);
            Assert.Equal('2', resultado.ColorRevelado);
            Assert.Null(resultado.MensajeTrampa);
            Assert.Equal(1, gestor.ConsultarUsosDisponibles("codigo-valido"));
        }
        finally
        {
            File.Delete(ruta);
        }
    }

    [Fact]
    public void EvaluarIntento_PosicionYaCorrecta_DevuelveSinSuerteYConsumeUso()
    {
        var ruta = CrearArchivo("""
            [
              { "hash": "codigo-usado", "usosDisponibles": 2 },
              { "hash": "otro-codigo", "usosDisponibles": 3 }
            ]
            """);

        try
        {
            var gestor = new GestorCodigosTrampa(ruta);
            var evaluador = CrearEvaluador(gestor, new RandomFijo(0));

            var primerResultado = evaluador.EvaluarIntento("1234", "1234", "codigo-usado");
            var segundoResultado = evaluador.EvaluarIntento("1234", "1234", "otro-codigo");

            Assert.True(primerResultado.CodigoTrampaValido);
            Assert.Equal(1, primerResultado.PosicionRevelada);
            Assert.Null(primerResultado.ColorRevelado);
            Assert.Equal("sin suerte", primerResultado.MensajeTrampa);
            Assert.Equal(1, gestor.ConsultarUsosDisponibles("codigo-usado"));

            Assert.False(segundoResultado.CodigoTrampaValido);
            Assert.Contains("Ya se utilizó", segundoResultado.MensajeTrampa);
            Assert.Equal(3, gestor.ConsultarUsosDisponibles("otro-codigo"));
        }
        finally
        {
            File.Delete(ruta);
        }
    }

    [Theory]
    [InlineData("inexistente")]
    [InlineData("agotado")]
    public void EvaluarIntento_CodigoInvalido_NoConsumeNiMarcaLaPartida(string cheatCode)
    {
        var ruta = CrearArchivo("""
            [
              { "hash": "agotado", "usosDisponibles": 0 },
              { "hash": "disponible", "usosDisponibles": 1 }
            ]
            """);

        try
        {
            var gestor = new GestorCodigosTrampa(ruta);
            var evaluador = CrearEvaluador(gestor, new RandomFijo(0));

            var resultadoInvalido = evaluador.EvaluarIntento("1234", "4321", cheatCode);
            var resultadoValido = evaluador.EvaluarIntento("1234", "4321", "disponible");

            Assert.False(resultadoInvalido.CodigoTrampaValido);
            Assert.Contains("no existe o no tiene usos", resultadoInvalido.MensajeTrampa);
            Assert.True(resultadoValido.CodigoTrampaValido);
            Assert.Equal(0, gestor.ConsultarUsosDisponibles("disponible"));
            Assert.Equal(0, gestor.ConsultarUsosDisponibles("agotado"));
        }
        finally
        {
            File.Delete(ruta);
        }
    }

    private static Evaluador CrearEvaluador(GestorCodigosTrampa gestor, Random random)
    {
        return new Evaluador(
            logEvaluaciones: new RegistradorNulo(),
            gestorCodigosTrampa: gestor,
            random: random);
    }

    private static string CrearArchivo(string contenido)
    {
        var ruta = Path.Combine(AppContext.BaseDirectory, $"{Guid.NewGuid():N}.json");
        File.WriteAllText(ruta, contenido);
        return ruta;
    }

    private sealed class RegistradorNulo : ILogEvaluaciones
    {
        public void Registrar(string mensaje)
        {
        }
    }

    private sealed class RandomFijo : Random
    {
        private readonly int _indice;

        public RandomFijo(int indice)
        {
            _indice = indice;
        }

        public override int Next(int maxValue) => _indice;
    }
}