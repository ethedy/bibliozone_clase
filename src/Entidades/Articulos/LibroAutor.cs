using System;
using System.Collections.Generic;
using System.Text;

namespace Entidades.Articulos
{
  public class LibroAutor
  {
    public Guid ID_Autor { get; set; }

    public int ID_Libro { get; set; }

    public Autor Autor { get; set; }

    public Libro Libro { get; set; }
  }
}
