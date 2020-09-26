using System;

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
  /// </summary>
  public class Libro
  {
    //  TODO agregar las propiedades restantes para completar el modelo de dominio de Libro

    public string ID { get; set; }

    public string ISBN10 { get; set; }

    public string Titulo { get; set; }

    public int? Paginas { get; set; }

    /// <summary>
    /// Fecha de Publicacion, si solo tenemos el año se toma como fecha el 1 de enero
    /// </summary>
    public DateTime? Publicacion { get; set; }

    public decimal? Precio { get; set; }

    public TipoDePublico? Publico { get; set; }
  }
}
