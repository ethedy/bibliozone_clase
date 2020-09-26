using System;
using System.Text;
using Entidades.Articulos;
using Servicios;

namespace console
{
  class Program
  {
    static void Main(string[] args)
    {
      ServiciosImportacion importacion = default;

      try
      {
        importacion = new ServiciosImportacion(@"D:\CURSOS\INCOMPANY\clase\datos\libros.csv");

        var lista = importacion.ImportarCSV();

        //  Observar que si uso la version IEnumerable<T> no puedo acceder al resultado por subindices!!
        //
        //  for (int idx = 0; idx < lista.Count; idx++)
        //  {
        //    Console.WriteLine($"Titulo: {lista[idx].Titulo}");
        //  }

        foreach (Libro libro in lista)
        {
          Console.WriteLine($"Titulo: {libro?.Titulo}");
        }
        //  guardar en base de datos....o no,,,

      }
      catch (ApplicationException ex) when( ex.Data.Contains("archivo"))
      {
        importacion?.ImportarCSV();

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
        Console.WriteLine("[finally] ==> programa terminado!!");
      }

      Console.WriteLine("Presionar ENTER");
      Console.ReadLine();
    }
  }
}
