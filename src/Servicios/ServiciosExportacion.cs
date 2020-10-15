using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Datos;
using Entidades.Articulos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Servicios
{
  public class ServiciosExportacion
  {
    private readonly ExportContext _ctx;

    private readonly ILogger<ServiciosExportacion> _logger;

    public ServiciosExportacion(ExportContext ctx, ILogger<ServiciosExportacion> logger)
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

    /// <summary>
    /// Este metodo por supuesto que es solo para testeos...no es sano eliminar los datos de las tablas de este modo
    /// </summary>
    public void ClearDatabase()
    {
      _logger.LogWarning("Los datos de todas las tablas de articulos se eliminaran!!");

      _ctx.Database.ExecuteSqlRaw("delete from Libros_Autores; delete from Autores; delete from Libros;");
    }

    /// <summary>
    /// Ejemplo de reemplazo para la funcionalidad de EF6 SqlQuery&lt;T>
    /// </summary>
    /// <param name="editorial"></param>
    /// <returns></returns>
    public List<string> ObtenerTitulosDeEditorial(string editorial)
    {
      return _ctx
        .Set<Libro>()
        .FromSqlInterpolated($"select * from Libros where Editorial={editorial}")
        .Select(l => l.Titulo)
        .ToList();
    }

    public IEnumerable<Libro> GetLibros(string titulo)
    {
      return _ctx
        .Set<Libro>()
        .Where(lib => lib.Titulo.Contains(titulo))
        .Include(lib => lib.LibroAutores)
        .ThenInclude(la => la.Autor)
        .AsEnumerable();
    }

    public Autor GetAutor(string nombre)
    {
      return _ctx.Autores
        .Include(au => au.AutorLibros)
        .ThenInclude(al => al.Libro)
        .FirstOrDefault(aut => aut.Nombre == nombre);
    }

    /// <summary>
    /// Agrega un libro nuevo sin pasar por la importacion
    /// </summary>
    /// <param name="nuevo"></param>
    public void AgregarLibro(Libro nuevo, bool save = true)
    {
      //_ctx.Debug();

      if (_ctx.Find<Libro>(nuevo.ID_Real) != null)
        _ctx.Update(nuevo);
      else
        _ctx.Add(nuevo);

      //_ctx.Debug();

      if (save)
        _ctx.SaveChanges();
    }

    /// <summary>
    /// Agrega un libro nuevo sin pasar por la importacion
    /// </summary>
    /// <param name="nuevo"></param>
    public void AgregarLibroAutor(LibroAutor nuevo, bool save = true)
    {
      if (_ctx.Find<LibroAutor>(nuevo.ID_Libro, nuevo.ID_Autor) != null)
        _ctx.Update(nuevo);
      else
        _ctx.Add(nuevo);

      //  _ctx.Debug();

      if (save)
        _ctx.SaveChanges();
    }
  }
}
