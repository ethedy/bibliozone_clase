#undef RUN_PRUEBAS

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Entidades.Articulos;
using Figgle;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PromptSharp;
using Servicios;
using Utiles;

namespace console
{
  public class Aplicacion
  {
    private readonly ServiciosImportacion _imp;

    private readonly ServiciosExportacion _exp;

    private readonly IConfiguration _config;

    private readonly ILogger<Aplicacion> _logger;

    public Aplicacion(ServiciosImportacion imp, ServiciosExportacion exp, IConfiguration config, ILogger<Aplicacion> logger)
    {
      _imp = imp;
      _exp = exp;
      _config = config;
      _logger = logger;
    }

    public void Run()
    {
      Console.Clear();
      Header("biblioZ-cli", "Interface de linea de comando para BiblioZone");

      try
      {
        int opcion = Prompt.Menu("Seleccionar una de las acciones que queres realizar", new[]
        {
          "Importar Libros desde --file y ejecutar las pruebas en memoria",
          "Importar Libros desde --file y guardar en la DB",
          "*** Importar Autores desde --file [falta implementar guardar en la DB]",
          "Consultas varias desde el contexto",
          "Pruebas ingresando Libros"
        });

        //  TODO_HECHO obtener archivo desde la configuracion
        //
        //  --file=D:\CURSOS\INCOMPANY\clase\datos\libros.csv --tipo=libros
        //
        //  --file=D:\CURSOS\INCOMPANY\clase\datos\autores.csv --tipo=autores
        //
        //  NOTA: cuando ponemos el menu, no tiene sentido usar la opcion --tipo
        //
        string file = _config["file"];

        if (opcion.In(0, 2, 1) && file == null)
          throw new ApplicationException("La opcion seleccionada necesita que se pase un archivo en --file");

        switch (opcion)
        {
          case 0:
            {
              if (Prompt.Confirm($"Es el archivo correcto? ==> {file}"))
              {
                _logger.LogInformation("Iniciando el procesamiento del archivo de Libros {archivo}", file);

                IEnumerable<Libro> lista = _imp.ImportarCSV(file);

                _logger.LogInformation("Ejecutando pruebas en memoria...");

                //  ejecutamos las pruebas sobre la lista importada (memoria)
                //
                Pruebas(lista);
              }
            }
            break;

          case 1:
            {
              if (Prompt.Confirm($"Es el archivo correcto? ==> {file}"))
              {
                _logger.LogInformation("Iniciando el procesamiento del archivo de Libros {archivo}", file);

                IEnumerable<Libro> lista = _imp.ImportarCSV(file);

                _logger.LogInformation("Iniciando el proceso de exportacion");

                //  eliminamos los datos previos??
                if (Prompt.Confirm("Eliminamos datos previos?", true,
                  "WARNING Esta operacion eliminara todos los datos de las 3 tablas de Articulos!!"))
                {
                  _exp.ClearDatabase();
                }

                //  pasamos la responsabilidad de la exportacion al componente adecuado...
                //
                _exp.ExportarListaDeLibros(lista);
              }
            }
            break;

          case 2:
            {
              if (Prompt.Confirm($"Es el archivo correcto? ==> {file}"))
              {
                _logger.LogInformation("Iniciando el procesamiento del archivo de Autores {archivo}", file);

                var autoresTemp = _imp.ImportarAutores(file);

                Console.WriteLine("Lista de autores importados...");

                foreach (var item in autoresTemp)
                {
                  Console.WriteLine($"Se importo el siguiente par (idLibro, autor) ==> {item}");

                  _logger.LogDebug("Se importo el siguiente par (idLibro, autor) ==> {tupla}", item);
                }
              }
            }
            break;

          case 3:
            {
              string editorial = Prompt.Input<string>("Ingresar el nombre de una editorial");

              Console.WriteLine($"Titulos para la Editorial {editorial}:\n");

              foreach (string titulo in _exp.ObtenerTitulosDeEditorial(editorial))
              {
                Console.WriteLine(titulo);
              }
            }
            break;

          case 4:
            {
              Console.WriteLine($"Pruebas de ingresos varias para libros y autores:\n");
              string titulo = Prompt.Input<string>("Ingresar titulo o parte del titulo");

              var libroResultado = _exp.GetLibros(titulo).FirstOrDefault();

              if (libroResultado == null)
              {
                //  crear nuevo libro...
                libroResultado = new Libro()
                {
                  ID = "1234",
                  Titulo = titulo,
                  Publicacion = null
                };
              }
              else
              {
                Console.WriteLine($"Autores del libro: {libroResultado.Titulo}");

                foreach (var au in libroResultado.LibroAutores)
                  Console.WriteLine($"ID={au.ID_Autor} ; Nombre={au.Autor.Nombre}");
              }

              string autor = Prompt.Input<string>("Ingresar nombre del autor (exacto)");
              var autorResultado = _exp.GetAutor(autor);

              if (autorResultado == null)
              {
                autorResultado = new Autor()
                {
                  Nombre = autor
                };
              }
              else
              {
                Console.WriteLine($"Libros escritos por {autor}");

                foreach (var li in autorResultado.AutorLibros)
                  Console.WriteLine($"{li.Libro.Titulo}");
              }

              //  PARA MEDITAR -- De que manera podemos evitar el UPDATE del Libro que no esta modificado??
              //
              libroResultado.LibroAutores.Add(new LibroAutor() { Libro = libroResultado, Autor = autorResultado });

              _exp.AgregarLibro(libroResultado);
              //  _exp.AgregarLibroAutor(new LibroAutor() { Libro = libroResultado, Autor = autorResultado });

              Console.WriteLine($"GUID: {libroResultado.ID_Real} Fecha: {libroResultado.Publicacion}");
            }
            break;
        }
      }
      catch (ApplicationException ex) when (ex.Data.Contains("archivo"))
      {
        Console.WriteLine($"Se produjo una excepcion {ex.Message} Archivo: {ex.Data["archivo"]}");
      }
      catch (NullReferenceException ex)
      {
        Console.WriteLine(ex.Message);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
      finally
      {
        Console.WriteLine();
        Console.WriteLine("[finally] ==> programa terminado!!");
      }
    }

    private void Pruebas(IEnumerable<Libro> lista)
    {
      //  Ejemplo #1
      //  CASTING
      //
      Casting_a_List_v1("Ejemplo #1", lista);

      PrintLista(lista, l => l.ID == "dummy", l => $"{l.ID} {l.Titulo}");

      //  Ejemplo #2
      //  CASTING
      //
      Casting_a_List_v2("Ejemplo #2", lista);

      PrintLista(lista, l => l.ID == "dummy", l => $"{l.ID} {l.Titulo}");

      //  Observar que si uso la version IEnumerable<T> no puedo acceder al resultado por subindices!!
      //
      //  for (int idx = 0; idx < lista.Count; idx++)
      //  {
      //    Console.WriteLine($"Titulo: {lista[idx].Titulo}");
      //  }

      //  Ejemplo #3
      //  DELEGADOS
      //
      Dos_Predicados_Con_Funciones_Locales("Ejemplo #3", lista);

      //  Ejemplo #4
      //  
      //
      Predicados_con_Funciones_Locales_y_Where("Ejemplo #4", lista);

      //  Ejemplo #5
      //
      //
      Expresiones_Lambda("Ejemplo #5", lista);


      //  Ejemplo #6
      //
      //
      var tupla = RangoPrecios("Ejemplo #6", lista);

      Console.WriteLine($"MIN = {tupla.min} MAX = {tupla.max}");


      //  Ejemplo #7
      //
      //
      Proyeccion_con_Filtro("Ejemplo #7", lista);

      //PrintLista(lista, l => l.ID == "dummy", l => $"{l.ID} {l.Titulo} {l.Precio}");
    }

    //
    //  version 1: segura, si el casting no se puede hacer, lista es null.
    //  Probar cambiando el tipo base en ImportarCSV
    //
    private void Casting_a_List_v1(string titulo, IEnumerable<Libro> ienum)
    {
      Console.WriteLine(
        $"\n===={titulo}=======================================================================================");
      Console.WriteLine(
        "Parte 1: segura y compacta. Si el casting no se puede hacer, lista es null ==> no hay resultados");
      Console.WriteLine(
        "Parte 2: segura pero mas codigo. Usa el operador is, si la condicion se cumple uso casting tradicional");
      Console.WriteLine(
        "Para probar condicion de error tenemos que poner en ServiciosImportacion.cs #define RETORNA_ARRAY");
      Console.WriteLine();

      //  operador is con pattern matching C# 7 (declara y asigna la variable lista) ==> MEJOR OPCION
      //
      //  https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-7#pattern-matching
      //
      if (ienum is List<Libro> lista)
      {
        lista[0] = new Libro()
        { ID = "dummy", Titulo = "La Biblia para Programadores Agnosticos", Paginas = 1200, Precio = 100.0M };
      }

      //  operador is y casting tradicional
      //
      if (ienum is List<Libro>)
      {
        List<Libro> mismaLista = (List<Libro>)ienum;

        mismaLista[20].ID = "dummy";
        mismaLista[20].Titulo = "Programacion Orientada a Objetos en la Edad Media";
      }
    }

    //
    //  version 2: INSEGURA, si el casting no se puede hacer, se lanza una excepcion.
    //  Probar cambiando el tipo base en ImportarCSV
    //
    private void Casting_a_List_v2(string titulo, IEnumerable<Libro> ienum)
    {
      Console.WriteLine(
        $"\n===={titulo}=======================================================================================");
      Console.WriteLine("Parte 1: INSEGURA ==> Si el casting tradicional no se puede hacer, se lanza una excepcion");
      Console.WriteLine("Parte 2: segura, mas codigo. Usa operador as y chequeo contra null de la instancia");
      Console.WriteLine(
        "Para probar condicion de error tenemos que poner en ServiciosImportacion.cs #define RETORNA_ARRAY");
      Console.WriteLine();

      //  casting tradicional falla con excepcion si no es del tipo correcto
      //
      try
      {
        List<Libro> lista = (List<Libro>)ienum;

        lista[0] = new Libro()
        { ID = "dummy", Titulo = "La Biblia para Programadores Agnosticos", Paginas = 1200, Precio = 100.0M };
      }
      catch (InvalidCastException ex)
      {
        Console.WriteLine("CAST INVALIDO!!");
      }

      List<Libro> otraLista = ienum as List<Libro>;

      if (otraLista != null)
      {
        otraLista[20].ID = "dummy";
        otraLista[20].Titulo = "Programacion Orientada a Objetos en la Edad Media";
      }
      else
        Console.WriteLine("NO SE PUDO REALIZAR EL CASTING!!");
    }


    private void Dos_Predicados_Con_Funciones_Locales(string titulo, IEnumerable<Libro> ienum)
    {
      Console.WriteLine(
        $"\n===={titulo}=======================================================================================");
      Console.WriteLine("Usamos dos funciones locales y un foreach rudo para filtrar la lista");
      Console.WriteLine("Declaramos un delegado y le asignamos cada funcion cuando la necesitamos");
      Console.WriteLine();

      Func<Libro, bool> predicado = FiltroTexto;
      int paginas = 600;

      foreach (Libro libro in ienum)
      {
        if (predicado(libro))
          Console.WriteLine($"Titulo: {libro?.Titulo} {libro.Paginas}");
      }

      Console.WriteLine("\n>>>>>>>>  CAMBIO PREDICADO  <<<<<<<<\n");

      predicado = FiltroPaginas;

      foreach (Libro libro in ienum)
      {
        if (predicado(libro))
          Console.WriteLine($"Titulo: {libro?.Titulo} {libro.Paginas}");
      }

      #region FUNCIONES LOCALES

      bool FiltroPaginas(Libro item)
      {
        return item.Paginas > paginas;
      }

      bool FiltroTexto(Libro item)
      {
        return item.Titulo.Contains("Boot") || item.Titulo.Contains("Agnos");
      }

      #endregion
    }

    private void Predicados_con_Funciones_Locales_y_Where(string titulo, IEnumerable<Libro> ienum)
    {
      Console.WriteLine(
        $"\n===={titulo}=======================================================================================");
      Console.WriteLine("Usamos dos funciones locales como predicados de dos Where de Enumerable");
      Console.WriteLine("Se encadenan mediante fluent syntax. Se muestra el resultado (AND de los predicados)");
      Console.WriteLine();

      int paginas = 200;
      Func<Libro, bool> predicado = FiltroPaginas;

      foreach (Libro libro in ienum.Where(predicado).Where(FiltroTexto))
        Console.WriteLine($"Titulo: {libro?.Titulo} {libro.Paginas}");

      #region FUNCIONES LOCALES

      bool FiltroPaginas(Libro item)
      {
        return item.Paginas > paginas;
      }

      bool FiltroTexto(Libro item)
      {
        return item.Titulo.Contains("Boot") || item.Titulo.Contains("Agnos");
      }

      #endregion
    }

    /// <summary>
    /// Eliminamos todas las funciones locales reemplazandolas por expresiones lambda en una unica expresion
    /// </summary>
    /// 
    /// <code>
    /// <![CDATA[
    /// class Enumerable ==> MEMORIA
    ///
    ///  Func<Libro, bool> predicado = FiltroPaginas;
    ///
    ///  f(x) = x * 10
    ///  
    ///  int XPor10(int x) { return x * 10; }
    ///
    ///  (x) => x * 10
    ///
    ///  class Queryable ==> DB
    /// ]]>
    /// </code>
    /// <param name="titulo"></param>
    /// <param name="lista"></param>
    private void Expresiones_Lambda(string titulo, IEnumerable<Libro> lista)
    {
      Console.WriteLine(
        $"\n===={titulo}=======================================================================================");
      Console.WriteLine(
        "Eliminamos todas las funciones locales reemplazandolas por expresiones lambda en una unica expresion");
      Console.WriteLine();

      var listaFiltrada =
        lista.Where(p => p.Paginas > 200 && (p.Titulo.Contains("Boot") || p.Titulo.Contains("Agnos")));

      foreach (var libro in listaFiltrada)
        Console.WriteLine($"Titulo: {libro?.Titulo} {libro.Paginas}");
    }

    private (decimal min, decimal max) RangoPrecios(string titulo, IEnumerable<Libro> libros)
    {
      Console.WriteLine(
        $"\n===={titulo}=======================================================================================");
      Console.WriteLine("Obtener rango de precios minimo y maximo de la coleccion. Retornar una TUPLA");
      Console.WriteLine(
        "Usamos dos metodos de extension agregados Min y Max, previo asegurarnos que la coleccion no tenga");
      Console.WriteLine("nulos y ademas contenga elementos, de otra manera Min/Max producen una excepcion");
      Console.WriteLine();

      var noNulos = libros.Where(l => l.Precio != null);

      if (noNulos.Count() != 0)
      {
        decimal min = noNulos
          .Select(l => new { PrecioNoNull = l.Precio.Value })
          .Min(x => x.PrecioNoNull);

        decimal max = noNulos.Max(l => l.Precio.Value);

        return (min, max);
      }

      return (0, 0);
    }

    private void Proyeccion_con_Filtro(string titulo, IEnumerable<Libro> lista)
    {
      Console.WriteLine(
        $"\n===={titulo}=======================================================================================");
      Console.WriteLine(
        "Usamos condicion de filtro para la lista y luego proyectamos el resultado con Select para obtener un");
      Console.WriteLine("nuevo tipo de objeto (anonimo) que mostramos por pantalla. EL TIPO ANONIMO ES INMUTABLE");
      Console.WriteLine("WARNING! Asegurarse que en ServiciosImportacion.cs tenga #undef RETORNA_ARRAY");
      Console.WriteLine();

      var proyeccion = lista
        .Where(lib => lib.ID == "dummy")
        .Select(lib => new { lib.ID, lib.Titulo, PrecioIVA = (lib.Precio ?? 0) * 1.21M, lib.Precio });

      foreach (var s in proyeccion)
      {
        Console.WriteLine($"Titulo: {s.Titulo} {s.ID} {s.PrecioIVA} {s.Precio}");

        //  OJO!!! Esto NO se puede hacer por la definicion del tipo anonimo!!!
        //
        //  s.Titulo = "Nuevo Titulo";
      }
    }

    private void PrintLista(IEnumerable<Libro> lista, Func<Libro, bool> predicado, Func<Libro, string> toString)
    {
      //Console.WriteLine("---------------------------------------------------------------------------------------");

      foreach (var item in lista.Where(predicado))
        Console.WriteLine(toString(item));

      Console.WriteLine();
      Console.WriteLine();
    }

    private void Header(string header, string subtitulo = null)
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine(FiggleFonts.Standard.Render(header));
      //Console.WriteLine(FiggleFonts.Roman.Render("Actualiza"));
      if (subtitulo != null)
      {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(subtitulo);
      }
      Console.ForegroundColor = ConsoleColor.White;
      Console.WriteLine();
    }

    private void Footer(string footer)
    {
      Console.ForegroundColor = ConsoleColor.DarkGreen;
      Console.WriteLine(footer);
      Console.ForegroundColor = ConsoleColor.White;
      Console.WriteLine();
    }
  }
}
