namespace Mastermind.Core.Tests;

public class EvaluadorTests
{
    [Fact]
    public void EvaluarIntento_PatronIgualAlIntento_DevuelveTodoBienPosicionado()
    {
        var evaluador = CrearEvaluadorConLetras();

        var resultado = evaluador.EvaluarIntento("RGBY", "RGBY");

        Assert.Equal(new ResultadoEvaluacion(4, 0), resultado);
    }

    [Theory]
    [InlineData("RGBY", "RBYG", 1, 3)]
    [InlineData("RRGB", "RGRR", 1, 2)]
    public void EvaluarIntento_CoincidenciasParciales_DevuelveConteosCorrectos(
        string patronSecreto,
        string intentoJugador,
        int bienPosicionados,
        int colorCorrecto)
    {
        var evaluador = CrearEvaluadorConLetras();

        var resultado = evaluador.EvaluarIntento(patronSecreto, intentoJugador);

        Assert.Equal(new ResultadoEvaluacion(bienPosicionados, colorCorrecto), resultado);
    }

    [Fact]
    public void EvaluarIntento_SinCoincidencias_DevuelveCero()
    {
        var evaluador = CrearEvaluadorConLetras();

        var resultado = evaluador.EvaluarIntento("ABCD", "WXYZ");

        Assert.Equal(new ResultadoEvaluacion(0, 0), resultado);
    }

    [Theory]
    [InlineData("RRGG", "RRRR", 2, 0)]
    [InlineData("RRGB", "RGRR", 1, 2)]
    public void EvaluarIntento_LetrasRepetidasEnAmbosCodigos_NoCuentaDeMas(
        string patronSecreto,
        string intentoJugador,
        int bienPosicionados,
        int colorCorrecto)
    {
        var evaluador = CrearEvaluadorConLetras();

        var resultado = evaluador.EvaluarIntento(patronSecreto, intentoJugador);

        Assert.Equal(new ResultadoEvaluacion(bienPosicionados, colorCorrecto), resultado);
    }

    [Theory]
    [InlineData("ABCD", "DXXX", 0, 1)]
    [InlineData("ABCD", "AABC", 1, 2)]
    public void EvaluarIntento_LetrasRepetidasSoloEnIntento_NoExcedeFrecuenciaSecreta(
        string patronSecreto,
        string intentoJugador,
        int bienPosicionados,
        int colorCorrecto)
    {
        var evaluador = CrearEvaluadorConLetras();

        var resultado = evaluador.EvaluarIntento(patronSecreto, intentoJugador);

        Assert.Equal(new ResultadoEvaluacion(bienPosicionados, colorCorrecto), resultado);
    }

    [Fact]
    public void EvaluarIntento_SinConfiguracion_UsaPaletaPredeterminada()
    {
        var evaluador = new Evaluador();

        var resultado = evaluador.EvaluarIntento("1234", "4321");

        Assert.Equal(new ResultadoEvaluacion(0, 4), resultado);
    }

    [Theory]
    [InlineData(null, "1234", "patronSecreto")]
    [InlineData("1234", null, "intentoJugador")]
    public void EvaluarIntento_EntradaNula_LanzaArgumentNullException(
        string? patronSecreto,
        string? intentoJugador,
        string parametroEsperado)
    {
        var evaluador = new Evaluador();

        var excepcion = Assert.Throws<ArgumentNullException>(
            () => evaluador.EvaluarIntento(patronSecreto!, intentoJugador!));

        Assert.Equal(parametroEsperado, excepcion.ParamName);
        Assert.Contains("no puede ser nulo", excepcion.Message);
    }

    [Theory]
    [InlineData("", "1234", "patronSecreto")]
    [InlineData("1234", "", "intentoJugador")]
    public void EvaluarIntento_EntradaVacia_LanzaArgumentException(
        string patronSecreto,
        string intentoJugador,
        string parametroEsperado)
    {
        var evaluador = new Evaluador();

        var excepcion = Assert.Throws<ArgumentException>(
            () => evaluador.EvaluarIntento(patronSecreto, intentoJugador));

        Assert.Equal(parametroEsperado, excepcion.ParamName);
        Assert.Contains("vacío", excepcion.Message);
    }

    [Fact]
    public void EvaluarIntento_LongitudesDiferentes_LanzaArgumentException()
    {
        var evaluador = new Evaluador();

        var excepcion = Assert.Throws<ArgumentException>(
            () => evaluador.EvaluarIntento("1234", "12345"));

        Assert.Contains("misma longitud", excepcion.Message);
    }

    [Theory]
    [InlineData("123", "123")]
    [InlineData("1234567", "1234567")]
    public void EvaluarIntento_TamanoFueraDelRango_LanzaArgumentException(
        string patronSecreto,
        string intentoJugador)
    {
        var evaluador = new Evaluador();

        var excepcion = Assert.Throws<ArgumentException>(
            () => evaluador.EvaluarIntento(patronSecreto, intentoJugador));

        Assert.Contains("tamaño", excepcion.Message);
    }

    [Theory]
    [InlineData("12X4", "1234", "patrón secreto")]
    [InlineData("1234", "12?4", "intento del jugador")]
    public void EvaluarIntento_ColorNoConfigurado_LanzaArgumentException(
        string patronSecreto,
        string intentoJugador,
        string parteDelMensaje)
    {
        var evaluador = new Evaluador();

        var excepcion = Assert.Throws<ArgumentException>(
            () => evaluador.EvaluarIntento(patronSecreto, intentoJugador));

        Assert.Contains("no está configurado", excepcion.Message);
        Assert.Contains(parteDelMensaje, excepcion.Message);
    }

    [Fact]
    public void EvaluarIntento_ConfiguracionPersonalizada_AceptaLetrasDigitosYSimbolosYTamanoPropio()
    {
        var colores = new Dictionary<char, string>
        {
            ['a'] = "minúscula",
            ['Q'] = "mayúscula",
            ['7'] = "dígito",
            ['!'] = "símbolo"
        };
        var configuracion = new ConfiguracionColores(colores, tamanoMinimo: 3, tamanoMaximo: 4);
        var evaluador = new Evaluador(configuracion);

        var resultadoTamanoTres = evaluador.EvaluarIntento("aQ7", "7aQ");
        var resultadoTamanoCuatro = evaluador.EvaluarIntento("aQ7!", "!7Qa");

        Assert.Equal(new ResultadoEvaluacion(0, 3), resultadoTamanoTres);
        Assert.Equal(new ResultadoEvaluacion(0, 4), resultadoTamanoCuatro);
    }

    private static Evaluador CrearEvaluadorConLetras()
    {
        var colores = "RGBYABCDWXYZ"
            .Distinct()
            .ToDictionary(simbolo => simbolo, simbolo => simbolo.ToString());

        return new Evaluador(new ConfiguracionColores(colores));
    }
}