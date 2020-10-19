using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Servicios;

namespace web.Controllers
{
  public class VentasController : Controller
  {
    public VentasController(ServiciosExportacion serv)
    {

    }

    public IActionResult Index()
    {
      return View();
    }
  }
}
