using System.Globalization;

namespace Mastermind.Core;

public sealed class Evaluador
{
    private readonly ConfiguracionColores _configuracion;
    private readonly ILogEvaluaciones _logEvaluaciones;
    private readonly TimeProvider _timeProvider;
    private readonly GestorCodigosTrampa _gestorCodigosTrampa;
    private readonly Random _random;
    private bool _codigoTrampaUtilizado;

    public Evaluador(
        ConfiguracionColores? configuracion = null,
        ILogEvaluaciones? logEvaluaciones = null,
        TimeProvider? timeProvider = null,
        GestorCodigosTrampa? gestorCodigosTrampa = null,
        Random? random = null)
    {
        _configuracion = configuracion ?? new ConfiguracionColores();
        _logEvaluaciones = logEvaluaciones ?? new LogEvaluacionesConsola();
        _timeProvider = timeProvider ?? TimeProvider.System;
        _gestorCodigosTrampa = gestorCodigosTrampa ?? new GestorCodigosTrampa();
        _random = random ?? Random.Shared;
    }

    public ResultadoEvaluacion EvaluarIntento(
        string patronSecreto,
        string intentoJugador,
        string? cheatCode = null)
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

        var resultado = new ResultadoEvaluacion(bienPosicionados, coloresCorrectos);
        if (cheatCode is not null)
        {
            resultado = AplicarCodigoTrampa(cheatCode, patronSecreto, intentoJugador, resultado);
        }

        var mensaje = string.Format(
            CultureInfo.InvariantCulture,
            "En la fecha {0:dd-MM-yyyy HH:mm:ss} se evaluó el intento {1} contra el patrón {2}. Resultado: {3} bien posicionados – {4} con color correcto.",
            _timeProvider.GetLocalNow(),
            intentoJugador,
            patronSecreto,
            resultado.BienPosicionados,
            resultado.ColorCorrecto);

        _logEvaluaciones.Registrar(mensaje);
        return resultado;
    }

    private ResultadoEvaluacion AplicarCodigoTrampa(
        string cheatCode,
        string patronSecreto,
        string intentoJugador,
        ResultadoEvaluacion resultado)
    {
        if (_codigoTrampaUtilizado)
        {
            return resultado with
            {
                CodigoTrampaValido = false,
                MensajeTrampa = "Ya se utilizó un código de trampa en esta partida."
            };
        }

        if (!_gestorCodigosTrampa.IntentarConsumirUso(cheatCode))
        {
            return resultado with
            {
                CodigoTrampaValido = false,
                MensajeTrampa = "El código de trampa no existe o no tiene usos disponibles."
            };
        }

        _codigoTrampaUtilizado = true;
        var indice = _random.Next(patronSecreto.Length);
        if (patronSecreto[indice] == intentoJugador[indice])
        {
            return resultado with
            {
                CodigoTrampaValido = true,
                PosicionRevelada = indice + 1,
                MensajeTrampa = "sin suerte"
            };
        }

        return resultado with
        {
            CodigoTrampaValido = true,
            PosicionRevelada = indice + 1,
            ColorRevelado = patronSecreto[indice]
        };
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

public sealed record ResultadoEvaluacion(
    int BienPosicionados,
    int ColorCorrecto,
    bool? CodigoTrampaValido = null,
    int? PosicionRevelada = null,
    char? ColorRevelado = null,
    string? MensajeTrampa = null);