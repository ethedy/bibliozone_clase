using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Servicios;
using web.Models;

namespace web.Controllers
{
  public class HomeController : Controller
  {
    private readonly ILogger<HomeController> _logger;

    private readonly IConfiguration _config;

    private readonly ServiciosExportacion _exp;

    public HomeController(ILogger<HomeController> logger, IConfiguration config, ServiciosExportacion exp)
    {
      _logger = logger;
      _config = config;
      _exp = exp;
    }

    public IActionResult Index()
    {
      _logger.LogCritical("Probamos...{path}", HttpContext.Request.Path.Value);

      //  Para ver error 500 tipico ==> descomentar throw y usar /
      //  Para ver el middleware de manejo de excepciones ==> usar /home/index
      //
      //  Para ejecutar desde el IDE y que no se interrumpa el codigo...
      //  Vamos a Debug/Windows/Exception Settings
      //  Buscamos System.ApplicationException
      //  Boton derecho --> Show Columns/Additional Actions
      //  En la columna de acciones adicionales seleccionar "Continue when unhandled in user code"
      //
      //  throw new ApplicationException("EXCEPCION PROVOCADA!!");

      //  La variable dinamica termina usando el diccionario para guardar el contenido
      //
      ViewBag.Mensaje = "Hola!!!";

      ViewData["Mensaje"] = "Hola hola!!!";

      return View();
    }

    public IActionResult Prueba()
    {
      return View();
    }

    public IActionResult Libros()
    {
      return View();
    }

    public IActionResult Libros(string editorial)
    {
      return View();
    }


    public IActionResult Privacy()
    {
      return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}
