namespace Mastermind.Core.Tests;

public class EvaluadorTests
{
    [Fact]
    public void EvaluarIntento_PatronIgualAlIntento_DevuelveTodoBienPosicionado()
    {
        var evaluador = new Evaluador();

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
        var evaluador = new Evaluador();

        var resultado = evaluador.EvaluarIntento(patronSecreto, intentoJugador);

        Assert.Equal(new ResultadoEvaluacion(bienPosicionados, colorCorrecto), resultado);
    }

    [Fact]
    public void EvaluarIntento_SinCoincidencias_DevuelveCero()
    {
        var evaluador = new Evaluador();

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
        var evaluador = new Evaluador();

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
        var evaluador = new Evaluador();

        var resultado = evaluador.EvaluarIntento(patronSecreto, intentoJugador);

        Assert.Equal(new ResultadoEvaluacion(bienPosicionados, colorCorrecto), resultado);
    }
}