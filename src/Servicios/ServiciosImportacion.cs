﻿#undef RETORNA_ARRAY

using System;
using System.IO;
using Entidades.Articulos;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Utiles;


namespace Servicios
{
  /// <summary>
  /// Clase de importacion
  /// </summary>
  public class ServiciosImportacion
  {
    private string _fileName;

    private readonly IConfiguration _config;

    private readonly ILogger<ServiciosImportacion> _logger;

    /// <summary>
    /// Constructor del servicio de importacion por ahora a partir de un nombre de archivo
    /// </summary>
    public ServiciosImportacion(IConfiguration config, ILogger<ServiciosImportacion> logger)
    {
      _config = config;
      _logger = logger;
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
    /// <exception cref="ApplicationException">Si el archivo no existe</exception>
    public IEnumerable<Libro> ImportarCSV(string fileName)
    {
      //  TODO observar como inicializamos la propiedad Data en la misma sentencia en la que creamos la excepcion
      //  TODO observar el inicializador del diccionario (similar al que vimos en el array pero mas complejo)
      //
      //  string fileName = "";
      if (!File.Exists(fileName))
        throw new ApplicationException("El archivo no existe")
        {
          Data =
          {
            {"archivo", fileName}
          }
        };

      _fileName = fileName;

      //  procesar archivo
      //  obtener lista de libros
      //  retornar la lista
      using StreamReader rdr = new StreamReader(_fileName) ;

#if RETORNA_ARRAY
      Libro[] resultado = new Libro[100];

      int idx = 0;
#else
      List<Libro> resultado = new List<Libro>();
#endif

      int saltarLineas = _config.GetValue<int>("saltarLineas");
      string separador = _config["separador"] ?? throw new ApplicationException("Por favor colocar un separador!!!");
      string[] formatos = _config.GetSection("formatosFecha").Get<string[]>();

      //  Libro[] resultado = new Libro[100];
      //  int idx = 0;
      bool libroValido;

      while (!rdr.EndOfStream)
      {
        if (saltarLineas != 0)
        {
          rdr.ReadLine();
          
          //  saltarLineas -= 1;
          //  saltarLineas = saltarLineas - 1;
          //  saltarLineas--;
          //
          saltarLineas--;

          continue;
        }

        string linea = rdr.ReadLine();
        //  char[] separadores = new char[2]; separadores[0] = ';'; separadores[1] = '|';
        string[] campos = linea?.Split(new[] {separador}, StringSplitOptions.None);

        if (campos?.Length == 19)
        {
          Libro nuevo = new Libro();
          libroValido = true;

          //  minimamente pedimos que el ID y el titulo no esten vacios!
          //
          nuevo.ID = campos[0].ActionIfEmpty(() => libroValido = false);
          nuevo.ISBN13 = campos[1].NullIfEmpty();
          nuevo.ISBN10 = campos[2].NullIfEmpty();
          nuevo.Titulo = campos[3].ActionIfEmpty(() => libroValido = false);
          nuevo.Subtitulo = campos[4].NullIfEmpty();
          nuevo.Editorial = campos[7].NullIfEmpty();
          nuevo.Descripcion = campos[9].NullIfEmpty();
          nuevo.Categoria = campos[10].NullIfEmpty();
          nuevo.Moneda = campos[12].NullIfEmpty();
          nuevo.Idioma = campos[15].NullIfEmpty();
          nuevo.LinkCanonico = campos[16].NullIfEmpty();
          nuevo.LinkImagen = campos[17].NullIfEmpty();
          nuevo.LinkInfo = campos[18].NullIfEmpty();

          //  TODO_HECHO procesar resto de los campos de texto

          //  Procesamos campo fecha_publicacion
          //  TODO_HECHO identificar por un lado formato incorrecto y por el otro ausencia de valor. En ambos casos el resultado es null pero si el formato es incorrecto debemos escribir al LOG
          //
          nuevo.Publicacion = null;
          if (!String.IsNullOrWhiteSpace(campos[5]))
          {
            if (DateTime.TryParseExact(campos[5], formatos, null,
              DateTimeStyles.None, out DateTime fechaTemp))
            {
              nuevo.Publicacion = fechaTemp;
            }
            else
              _logger.LogCritical("Formato de fecha incorrecto. Se recibe: {fecha}", campos[5]);

            //  Console.WriteLine($"WARNING: formato incorrecto!! Recibido: {campos[5]}");   //  TODO agregar LOG
          }

          nuevo.Publico = campos[8].ToUpper() switch
          {
            "NOT_MATURE" => TipoDePublico.TodoPublico,
            "MATURE" => TipoDePublico.Adulto,
            _ => null
          };

          //  TODO_HECHO procesar campo paginas
          //
          nuevo.Paginas = ParseEntero(campos[6], nameof(nuevo.Paginas));

          //  TODO_HECHO procesar campo precio
          //
          nuevo.Precio = null;
          if (!string.IsNullOrWhiteSpace(campos[11]))
          {
            if (Decimal.TryParse(campos[11], NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat,
              out decimal precio))
            {
              nuevo.Precio = precio;
            }
            else
              Console.WriteLine(
                $"LOG ERROR: Error no se puede convertir {campos[11]} en decimal. Propiedad {nameof(nuevo.Precio)}");
          }

          nuevo.Comentarios = ParseEntero(campos[14], nameof(nuevo.Comentarios));

          //  TODO_HECHO procesar campo rating_avg
          //  Solamente proceso el rating si los comentarios son diferentes de cero...de otra manera dejo null
          //
          nuevo.RatingPromedio = null;
          if (nuevo.Comentarios != 0)
          {
            if (!string.IsNullOrWhiteSpace(campos[13]))
            {
              if (Single.TryParse(campos[13], NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat,
                out float rating))
              {
                nuevo.RatingPromedio = rating;
              }
              else
                Console.WriteLine(
                  $"LOG ERROR: Error no se puede convertir {campos[13]} en float. Propiedad {nameof(nuevo.RatingPromedio)}");
            }
          }
          //
          //  nuevo.Precio = null;
          //  nuevo.Publicacion = null;

#if RETORNA_ARRAY
          if (libroValido)
            resultado[idx++] = nuevo;
#else
          if (libroValido)
            resultado.Add(nuevo);
#endif
          //  resultado[idx++] = nuevo;
        }
        else
        {
          Console.WriteLine($"LOG ERROR - {linea}");    //  TODO agregar LOG
        }
      }
      //  TODO Probar descomentar la siguiente excepcion para ver el comportamiento de los bloques catch y el filtro when
      //
      //  throw new ApplicationException("DUMMY");

#if RETORNA_ARRAY
      //  hacemos una copia superficial del array para no tener problemas de elementos null 
      //  
      Libro[] resultadoFinal = new Libro[idx];

      Array.Copy(resultado, resultadoFinal, idx);

      return resultadoFinal;
#else
      return resultado;
#endif


#region FUNCIONES LOCALES

      /*
       *  FUNCIONES LOCALES - NUEVA FEATURE DE C# 7
       *  https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-7#local-functions
       *
       */
      int? ParseEntero(string texto, string nombre)
      {
        int? result = null;

        if (!string.IsNullOrWhiteSpace(texto))
        {
          if (Int32.TryParse(texto, out int valor))
          {
            result = valor;
          }
          else
            Console.WriteLine($"LOG ERROR: el campo {nombre} no se puede convertir desde {texto} a entero");
        }

        return result;
      }

#endregion
    }

    /// <summary>
    /// Permite importar la lista de autores con sus libros asociados
    /// Retorna una enumeracion de tuplas con el id del libro y el nombre del autor
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public IEnumerable<(string idLibro, string nombre)> ImportarAutores(string fileName)
    {
      if (!File.Exists(fileName))
        throw new ApplicationException("El archivo no existe")
        {
          Data =
          {
            {"archivo", fileName}
          }
        };

      using StreamReader rdr = new StreamReader(fileName);

      List<(string id, string nombre)> resultado = new List<(string id, string nombre)>();

      int saltarLineas = _config.GetValue<int>("saltarLineas");
      string separador = _config["separador"] ?? ";";

      //  Libro[] resultado = new Libro[100];
      //  int idx = 0;

      while (!rdr.EndOfStream)
      {
        if (saltarLineas != 0)
        {
          rdr.ReadLine();

          saltarLineas--;

          continue;
        }

        string linea = rdr.ReadLine();

        string[] campos = linea?.Split(new[] { separador }, StringSplitOptions.None);

        if (campos?.Length == 2)
        {
          (string id, string aut) nuevo = default;

          nuevo.id = campos[0];
          nuevo.aut = campos[1];

          resultado.Add(nuevo);
        }
        else
        {
          Console.WriteLine($"LOG ERROR - {linea}"); //  TODO agregar LOG
        }
      }

      return resultado;
    }

  }
}

