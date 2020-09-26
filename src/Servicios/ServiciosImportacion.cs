using System;
using System.IO;
using Entidades.Articulos;
using System.Globalization;
using System.Collections.Generic;

namespace Servicios
{
  /// <summary>
  /// Clase de importacion
  /// </summary>
  public class ServiciosImportacion
  {
    private readonly string _fileName;

    /// <summary>
    /// Constructor del servicio de importacion por ahora a partir de un nombre de archivo
    /// </summary>
    /// <exception cref="ApplicationException">Si el archivo no existe</exception>
    /// <param name="fileName"></param>
    public ServiciosImportacion(string fileName)
    {
      //  TODO observar como inicializamos la propiedad Data en la misma sentencia en la que creamos la excepcion
      //  TODO observar el inicializador del diccionario (similar al que vimos en el array pero mas complejo)
      //
      if (!File.Exists(fileName))
        throw new ApplicationException("El archivo no existe")
        {
          Data =
          {
            {"archivo", fileName}
          }
        };

      _fileName = fileName;
    }

    /// <summary>
    ///     00  id                          -ta483U06R8C
    ///     01  isbn13                      9781430231486
    ///     02  isbn10                      1430231483
    ///     03  titulo                      Beginning Oracle Application Express 4
    ///     04  subtitulo                   From Novice to Professional
    ///     05  fecha_publicacion           2011-07-14
    ///     06  paginas                     440
    ///     07  editorial                   Apress
    ///     08  publico                     NOT_MATURE
    ///     09  descripcion                 Beginning Oracle Application Express 4 introduces one of the
    ///     10  categoria                   Computers
    ///     11  precio                      3355.04
    ///     12  moneda                      ARS
    ///     13  rating_avg                  4.5
    ///     14  rating_count                6
    ///     15  idioma                      en
    ///     16  canonical_link              https://play.google.com/store/books/details?id=-ta483U06R8C
    ///     17  imagen                      http://books.google.com/books/content?id=-ta483U06R8C&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api
    ///     18  self                        https://www.googleapis.com/books/v1/volumes/-ta483U06R8C.
    /// </summary>
    ///
    /// <returns>
    /// 
    /// </returns>

    public IEnumerable<Libro> ImportarCSV()
    {
      //  procesar archivo
      //  obtener lista de libros
      //  retornar la lista
      using StreamReader rdr = new StreamReader(_fileName) ;
      List<Libro> resultado = new List<Libro>();

      //  Libro[] resultado = new Libro[100];
      //  int idx = 0;

      while (!rdr.EndOfStream)
      {
        string linea = rdr.ReadLine();
        //  char[] separadores = new char[2]; separadores[0] = ';'; separadores[1] = '|';
        string[] campos = linea?.Split(new []{';'} , StringSplitOptions.None);

        if (campos?.Length == 19)
        {
          Libro nuevo = new Libro();

          nuevo.ID = campos[0];
          nuevo.ISBN10 = campos[2];
          nuevo.Titulo = campos[3];

          //  TODO procesar resto de los campos de texto

          //  procesamos campo fecha_publicacion
          //  TODO identificar por un lado formato incorrecto y por el otro ausencia de valor. En ambos casos el resultado es null pero si el formato es incorrecto debemos escribir al LOG
          //
          if (DateTime.TryParseExact(campos[5],new[] {"yyyy", "yyyy-MM-dd"}, null, 
              DateTimeStyles.None, out DateTime fechaTemp))
          {
            nuevo.Publicacion = fechaTemp;
          }
          else
            nuevo.Publicacion = null;

          nuevo.Publico = campos[8].ToUpper() switch
          {
            "NOT_MATURE" => TipoDePublico.TodoPublico,
            "MATURE" => TipoDePublico.Adulto,
            _ => null
          };

          //  TODO procesar campo paginas
          //  TODO procesar campo precio
          //  TODO procesar campo rating_avg

          //
          resultado.Add(nuevo);
          //  resultado[idx++] = nuevo;
        }
        else
        {
          Console.WriteLine($"LOG ERROR - {linea}");
        }
      }
      //  TODO Probar descomentar la siguiente excepcion para ver el comportamiento de los bloques catch y el filtro when
      //
      //  throw new ApplicationException("DUMMY");
      return resultado;
    }
  }
}

