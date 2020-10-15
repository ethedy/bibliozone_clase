using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace Entidades.Articulos
{
  public enum TipoDePublico
  {
    /// <summary>
    /// El material no es apto para ser consumido por menores de edad
    /// </summary>
    Adulto,

    /// <summary>
    /// El material es apto para todas las edades
    /// </summary>
    TodoPublico
  }

  /// <summary>
  /// Una publicacion que se puede vender dentro de la tienda
  /// 
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
  public class Libro
  {
    //  TODO_HECHO agregar las propiedades restantes para completar el modelo de dominio de Libro
    public Guid ID_Real { get; set; }

    public string ID { get; set; }

    public string ISBN13 { get; set; }

    public string ISBN10 { get; set; }

    public string Titulo { get; set; }

    public string Subtitulo { get; set; }

    /// <summary>
    /// Fecha de Publicacion, si solo tenemos el año se toma como fecha el 1 de enero
    /// </summary>
    public DateTime? Publicacion { get; set; }

    public int? Paginas { get; set; }

    public string Editorial { get; set; }

    public TipoDePublico? Publico { get; set; }

    public string Descripcion { get; set; }

    public string Categoria { get; set; }

    public decimal? Precio { get; set; }

    public string Moneda { get; set; }

    public float? RatingPromedio { get; set; }
    
    public int? Comentarios { get; set; }

    public string Idioma { get; set; }
    //  
    //
    public string LinkCanonico { get; set; }

    public string LinkImagen { get; set; }

    public string LinkInfo { get; set; }

    public ISet<LibroAutor> LibroAutores { get; set; }

    public Libro()
    {
      LibroAutores = new HashSet<LibroAutor>();
    }
  }
}
