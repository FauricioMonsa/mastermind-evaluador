using System.Collections.ObjectModel;

namespace Mastermind.Core;

public sealed class ConfiguracionColores
{
    private static readonly IReadOnlyDictionary<char, string> ColoresPredeterminados =
        new Dictionary<char, string>
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

    private readonly HashSet<char> _simbolosValidos;
    private readonly IReadOnlyDictionary<char, string> _colores;

    public IReadOnlyDictionary<char, string> Colores => _colores;

    public int TamanoMinimo { get; }

    public int TamanoMaximo { get; }

    public ConfiguracionColores(
        IReadOnlyDictionary<char, string>? colores = null,
        int tamanoMinimo = 4,
        int tamanoMaximo = 6)
    {
        if (tamanoMinimo <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(tamanoMinimo), "El tamaño mínimo debe ser mayor que cero.");
        }

        if (tamanoMaximo < tamanoMinimo)
        {
            throw new ArgumentOutOfRangeException(
                nameof(tamanoMaximo), "El tamaño máximo no puede ser menor que el mínimo.");
        }

        var coloresCopia = new Dictionary<char, string>(colores ?? ColoresPredeterminados);
        if (coloresCopia.Count == 0)
        {
            throw new ArgumentException("Debe configurarse al menos un color.", nameof(colores));
        }

        _colores = new ReadOnlyDictionary<char, string>(coloresCopia);
        _simbolosValidos = coloresCopia.Keys.ToHashSet();
        TamanoMinimo = tamanoMinimo;
        TamanoMaximo = tamanoMaximo;
    }

    internal bool ContieneSimbolo(char simbolo) => _simbolosValidos.Contains(simbolo);

    internal bool AceptaTamano(int tamano) => tamano >= TamanoMinimo && tamano <= TamanoMaximo;
}