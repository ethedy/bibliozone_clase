using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Datos;
using Entidades.Articulos;
using Microsoft.Extensions.Logging;

namespace Servicios
{
  public class ServiciosExportacion
  {
    private readonly BZoneContext _ctx;

    private readonly ILogger<ServiciosExportacion> _logger;

    public ServiciosExportacion(BZoneContext ctx, ILogger<ServiciosExportacion> logger)
    {
      _ctx = ctx;
      _logger = logger;
    }

    /// <summary>
    /// Aplica reglas de negocio a la lista de origen e intenta guardar la informacion faltante en la base de datos
    /// Solo incorpora libros que NO EXISTEN
    /// </summary>
    /// <param name="lista"></param>
    public void ExportarListaDeLibros(IEnumerable<Libro> lista)
    {
      //  ver si el libro ya existe...(por ISBN13)
      //  si el ISBN es null lo agrego sin mas...
      //  si no existe lo agrego

      foreach (Libro libro in lista)
      {
        //if (!_ctx.Libros.Any(lib => lib.ISBN13 == libro.ISBN13))
        if (!_ctx.Set<Libro>().Any(lib => lib.ISBN13 == libro.ISBN13))
        {
          //  podriamos setear la PK desde nuestra aplicacion!
          //
          //  nuevo.ID_Real = Guid.NewGuid();
          //
          _ctx.Add<Libro>(libro);
          //  _ctx.Libros.Add(libro);
        }
        else
          _logger.LogWarning("Se intento ingresar un elemento existente {libro}", libro);
      }
      //  guardamos la operacion TOTAL
      //
      _ctx.SaveChanges();
    }
  }
}
