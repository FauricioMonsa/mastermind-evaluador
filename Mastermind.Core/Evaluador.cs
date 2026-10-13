namespace Mastermind.Core;

public sealed class Evaluador
{
    private readonly ConfiguracionColores _configuracion;

    public Evaluador(ConfiguracionColores? configuracion = null)
    {
        _configuracion = configuracion ?? new ConfiguracionColores();
    }

    public ResultadoEvaluacion EvaluarIntento(string patronSecreto, string intentoJugador)
    {
        ValidarNoVacio(patronSecreto, nameof(patronSecreto), "El patrón secreto");
        ValidarNoVacio(intentoJugador, nameof(intentoJugador), "El intento del jugador");

        if (patronSecreto.Length != intentoJugador.Length)
        {
            throw new ArgumentException("El patrón secreto y el intento deben tener la misma longitud.");
        }

        if (!_configuracion.AceptaTamano(patronSecreto.Length))
        {
            throw new ArgumentException(
                $"El tamaño debe estar entre {_configuracion.TamanoMinimo} y {_configuracion.TamanoMaximo}.",
                nameof(patronSecreto));
        }

        ValidarColores(patronSecreto, "patrón secreto", nameof(patronSecreto));
        ValidarColores(intentoJugador, "intento del jugador", nameof(intentoJugador));

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

    private static void ValidarNoVacio(string? codigo, string parametro, string nombre)
    {
        if (codigo is null)
        {
            throw new ArgumentNullException(parametro, $"{nombre} no puede ser nulo.");
        }

        if (codigo.Length == 0)
        {
            throw new ArgumentException($"{nombre} no puede estar vacío.", parametro);
        }
    }

    private void ValidarColores(string codigo, string nombre, string parametro)
    {
        foreach (var simbolo in codigo)
        {
            if (!_configuracion.ContieneSimbolo(simbolo))
            {
                throw new ArgumentException(
                    $"El símbolo '{simbolo}' del {nombre} no está configurado.", parametro);
            }
        }
    }

    private static void IncrementarFrecuencia(Dictionary<char, int> frecuencias, char color)
    {
        frecuencias[color] = frecuencias.GetValueOrDefault(color) + 1;
    }
}

public sealed record ResultadoEvaluacion(int BienPosicionados, int ColorCorrecto);