using System;
using System.Collections.Generic;
using System.Text;
using Entidades.Articulos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Datos
{
  public class ConfigurarLibros : IEntityTypeConfiguration<Libro>
  {
    public void Configure(EntityTypeBuilder<Libro> builder)
    {
      //  por convencion, el nombre de la tabla es el nombre de la propiedad
      //
      builder.ToTable("Libros");

      //  si no declaramos la PK no va a leer el nuevo valor desde el proveedor de DB
      //
      builder.HasKey(lib => lib.ID_Real);

      builder.Property(lib => lib.ID_Real).HasColumnName("ID");
      builder.Property(lib => lib.ID).HasColumnName("Clave_Origen");
      builder.Property(lib => lib.Publicacion).HasColumnName("Fecha_Publicacion");
      builder.Property(lib => lib.Publico).HasColumnName("Tipo_Publico");
      builder.Property(lib => lib.RatingPromedio).HasColumnName("Promedio_Rating");

      //builder.Property(lib => lib.ID).HasColumnName("");
  
      //builder
      //  .HasOne(lib => lib.Editorial)
      //  .WithMany()
      //  .HasForeignKey("ID_Editorial");
    }
  }

  public class ConfigurarLibrosAutores : IEntityTypeConfiguration<LibroAutor>
  {
    public void Configure(EntityTypeBuilder<LibroAutor> builder)
    {
      builder.HasKey(la => new {la.ID_Libro, la.ID_Autor});


    }
  }
}
