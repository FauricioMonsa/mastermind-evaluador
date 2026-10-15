using System.Diagnostics;
using Mastermind.Core;
using Microsoft.AspNetCore.Mvc;
using Mastermind.Mvc.Models;

namespace Mastermind.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly Evaluador _evaluador;

    public HomeController(Evaluador evaluador)
    {
        _evaluador = evaluador;
    }

    public IActionResult Index()
    {
        return View(new EvaluacionViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(EvaluacionViewModel modelo)
    {
        try
        {
            modelo.Resultado = _evaluador.EvaluarIntento(
                modelo.PatronSecreto!,
                modelo.IntentoJugador!,
                string.IsNullOrWhiteSpace(modelo.CheatCode) ? null : modelo.CheatCode);
        }
        catch (ArgumentException excepcion)
        {
            ModelState.AddModelError(string.Empty, excepcion.Message);
        }

        return View(modelo);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
