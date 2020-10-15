using System;
using System.Collections.Generic;
using System.Text;

namespace Entidades.Articulos
{
  public class Autor
  {
    public int ID { get; set; }

    public string Nombre { get; set; }

    public string Biografia { get; set; }

    public ISet<LibroAutor> AutorLibros { get; set; }

    public Autor()
    {
      AutorLibros = new HashSet<LibroAutor>();
    }
  }
}
