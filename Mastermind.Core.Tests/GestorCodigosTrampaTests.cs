using System.Text.Json;

namespace Mastermind.Core.Tests;

public class GestorCodigosTrampaTests
{
    [Fact]
    public void GestorCodigosTrampa_SinRuta_LeeArchivoPredeterminadoCopiadoAlOutput()
    {
        var gestor = new GestorCodigosTrampa();

        Assert.Equal(1, gestor.ConsultarUsosDisponibles("A1B2C3D4E5F6G7H8I9J0ABCDE"));
        Assert.Equal(3, gestor.ConsultarUsosDisponibles("Z9Y8X7W6V5U4T3S2R1Q0LMNOP"));
    }

    [Fact]
    public void GestorCodigosTrampa_ConsumirUso_PersisteCantidadRestante()
    {
        var ruta = CrearArchivo("""
            [
              { "hash": "codigo-uno", "usosDisponibles": 1 },
              { "hash": "codigo-dos", "usosDisponibles": 3 }
            ]
            """);

        try
        {
            var gestor = new GestorCodigosTrampa(ruta);

            Assert.Equal(1, gestor.ConsultarUsosDisponibles("codigo-uno"));
            Assert.True(gestor.IntentarConsumirUso("codigo-uno"));
            Assert.Equal(0, gestor.ConsultarUsosDisponibles("codigo-uno"));
            Assert.False(gestor.IntentarConsumirUso("codigo-uno"));

            using var json = JsonDocument.Parse(File.ReadAllText(ruta));
            Assert.Equal(0, json.RootElement[0].GetProperty("usosDisponibles").GetInt32());
            Assert.Equal(3, json.RootElement[1].GetProperty("usosDisponibles").GetInt32());
        }
        finally
        {
            File.Delete(ruta);
        }
    }

    private static string CrearArchivo(string contenido)
    {
        var ruta = Path.Combine(AppContext.BaseDirectory, $"{Guid.NewGuid():N}.json");
        File.WriteAllText(ruta, contenido);
        return ruta;
    }
}