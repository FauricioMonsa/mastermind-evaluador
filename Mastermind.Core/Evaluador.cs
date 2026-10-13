namespace Mastermind.Core;

public sealed class Evaluador
{
    public ResultadoEvaluacion EvaluarIntento(string patronSecreto, string intentoJugador)
    {
        var bienPosicionados = 0;
        var coloresSecretosRestantes = new Dictionary<char, int>();
        var coloresIntentoRestantes = new Dictionary<char, int>();

        // Se cuentan y excluyen primero los aciertos exactos.
        for (var i = 0; i < patronSecreto.Length; i++)
        {
            var colorSecreto = patronSecreto[i];
            var colorIntento = intentoJugador[i];

            if (colorSecreto == colorIntento)
            {
                bienPosicionados++;
                continue;
            }

            IncrementarFrecuencia(coloresSecretosRestantes, colorSecreto);
            IncrementarFrecuencia(coloresIntentoRestantes, colorIntento);
        }

        // Solo se emparejan las frecuencias de los caracteres no exactos.
        var coloresCorrectos = 0;
        foreach (var (color, cantidadSecreta) in coloresSecretosRestantes)
        {
            if (coloresIntentoRestantes.TryGetValue(color, out var cantidadIntento))
            {
                coloresCorrectos += Math.Min(cantidadSecreta, cantidadIntento);
            }
        }

        return new ResultadoEvaluacion(bienPosicionados, coloresCorrectos);
    }

    private static void IncrementarFrecuencia(Dictionary<char, int> frecuencias, char color)
    {
        frecuencias[color] = frecuencias.GetValueOrDefault(color) + 1;
    }
}

public sealed record ResultadoEvaluacion(int BienPosicionados, int ColorCorrecto);