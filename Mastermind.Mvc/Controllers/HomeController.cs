using System.Diagnostics;
using Mastermind.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Mastermind.Mvc.Models;

namespace Mastermind.Mvc.Controllers;

public class HomeController : Controller
{
    private const string SessionEvaluatorKey = "Mastermind.EvaluadorKey";
    private const string CacheEvaluatorPrefix = "Mastermind.Evaluador.";
    private static readonly TimeSpan GameLifetime = TimeSpan.FromMinutes(30);
    private readonly IMemoryCache _memoryCache;

    public HomeController(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public IActionResult Index()
    {
        _ = ObtenerEvaluadorDePartida();
        return View(new EvaluacionViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(EvaluacionViewModel modelo)
    {
        try
        {
            modelo.Resultado = ObtenerEvaluadorDePartida().EvaluarIntento(
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult NuevaPartida()
    {
        var claveEvaluador = HttpContext.Session.GetString(SessionEvaluatorKey);
        if (claveEvaluador is not null)
        {
            _memoryCache.Remove(CacheEvaluatorPrefix + claveEvaluador);
        }

        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Index));
    }

    private Evaluador ObtenerEvaluadorDePartida()
    {
        var claveEvaluador = HttpContext.Session.GetString(SessionEvaluatorKey);
        if (claveEvaluador is not null &&
            _memoryCache.TryGetValue<Evaluador>(CacheEvaluatorPrefix + claveEvaluador, out var evaluador) &&
            evaluador is not null)
        {
            _memoryCache.Set(
                CacheEvaluatorPrefix + claveEvaluador,
                evaluador,
                new MemoryCacheEntryOptions { SlidingExpiration = GameLifetime });
            return evaluador;
        }

        claveEvaluador = Guid.NewGuid().ToString("N");
        evaluador = new Evaluador();
        _memoryCache.Set(
            CacheEvaluatorPrefix + claveEvaluador,
            evaluador,
            new MemoryCacheEntryOptions { SlidingExpiration = GameLifetime });
        HttpContext.Session.SetString(SessionEvaluatorKey, claveEvaluador);
        return evaluador;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
