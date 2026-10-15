using Mastermind.Core;

namespace Mastermind.Mvc.Models;

public sealed class EvaluacionViewModel
{
    public string? PatronSecreto { get; set; }

    public string? IntentoJugador { get; set; }

    public string? CheatCode { get; set; }

    public ResultadoEvaluacion? Resultado { get; set; }
}