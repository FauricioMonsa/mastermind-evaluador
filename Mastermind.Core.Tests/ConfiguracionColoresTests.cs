namespace Mastermind.Core.Tests;

public class ConfiguracionColoresTests
{
    [Fact]
    public void ConfiguracionColores_SinArgumentos_UsaPaletaYTamanosPredeterminados()
    {
        var configuracion = new ConfiguracionColores();
        var coloresEsperados = new Dictionary<char, string>
        {
            ['1'] = "Yellow",
            ['2'] = "Cyan",
            ['3'] = "Black",
            ['4'] = "Purple",
            ['5'] = "White",
            ['6'] = "Pink",
            ['7'] = "Red",
            ['8'] = "Brown",
            ['9'] = "Green"
        };

        Assert.Equal(coloresEsperados.Count, configuracion.Colores.Count);
        foreach (var (simbolo, nombre) in coloresEsperados)
        {
            Assert.Equal(nombre, configuracion.Colores[simbolo]);
        }

        Assert.Equal(4, configuracion.TamanoMinimo);
        Assert.Equal(6, configuracion.TamanoMaximo);
    }
}