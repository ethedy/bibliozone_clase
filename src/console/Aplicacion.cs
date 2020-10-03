using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Entidades.Articulos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Servicios;

namespace console
{
  public class Aplicacion
  {
    private readonly ServiciosImportacion _serv;

    public Aplicacion(ServiciosImportacion serv)
    {
      _serv = serv;
    }

    public void Run()
    {
      try
      {
        //  TODO obtener archivo desde la configuracion
        //
        IEnumerable<Libro> lista = _serv.ImportarCSV(@"D:\CURSOS\INCOMPANY\clase\datos\libros.csv");

        //  Ejemplo #1
        //  CASTING
        //
        Casting_a_List_v1("Ejemplo #1", lista);

        PrintLista(lista, l=>l.ID=="dummy", l=>$"{l.ID} {l.Titulo}");

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
        Proyeccion_con_Filtro("Ejemplo #7",  lista);

        //PrintLista(lista, l => l.ID == "dummy", l => $"{l.ID} {l.Titulo} {l.Precio}");

      }
      catch (ApplicationException ex) when (ex.Data.Contains("archivo"))
      {
        //  _serv.ImportarCSV("");

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
          {ID = "dummy", Titulo = "La Biblia para Programadores Agnosticos", Paginas = 1200, Precio = 100.0M};
      }

      //  operador is y casting tradicional
      //
      if (ienum is List<Libro>)
      {
        List<Libro> mismaLista = (List<Libro>) ienum;

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
        List<Libro> lista = (List<Libro>) ienum;

        lista[0] = new Libro()
          {ID = "dummy", Titulo = "La Biblia para Programadores Agnosticos", Paginas = 1200, Precio = 100.0M};
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
          .Select(l => new {PrecioNoNull = l.Precio.Value})
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
        .Select(lib => new {lib.ID, lib.Titulo, PrecioIVA = (lib.Precio ?? 0) * 1.21M, lib.Precio});

      foreach (var s in proyeccion)
      {
        Console.WriteLine($"Titulo: {s.Titulo} {s.ID} {s.PrecioIVA} {s.Precio}");

        //  OJO!!! Esto NO se puede hacer por la definicion del tipo anonimo!!!
        //
        //  s.Titulo = "Nuevo Titulo";
      }
    }

    private void PrintLista(IEnumerable<Libro> lista,Func<Libro, bool> predicado, Func<Libro, string> toString)
    {
      //Console.WriteLine("---------------------------------------------------------------------------------------");

      foreach (var item in lista.Where(predicado))
        Console.WriteLine(toString(item));

      Console.WriteLine();
      Console.WriteLine();
    }
  }
}
