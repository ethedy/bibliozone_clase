using System;
using System.Collections.Generic;
using System.Text;
using Entidades.Articulos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.Extensions.Configuration;

namespace Datos
{
  public class ConfigurarLibros : IEntityTypeConfiguration<Libro>
  {
    private readonly IConfiguration _config;

    public ConfigurarLibros(IConfiguration config)
    {
      _config = config;
    }

    public void Configure(EntityTypeBuilder<Libro> builder)
    {
      bool usarSqlDefaults = _config.GetValue<bool>("usarSqlDefaults");

      //  por convencion, el nombre de la tabla es el nombre de la propiedad
      //
      builder.ToTable("Libros");

      //  si no declaramos la PK no va a leer el nuevo valor desde el proveedor de DB
      //
      builder.HasKey(lib => lib.ID_Real);

      if (usarSqlDefaults)
      {
        //  el valor que ponga en el default de sql sirve para las migraciones (o sea si voy a generar el modelo en sql)
        //  newid() o newsequentialid() / getdate()
        //  si no uso migraciones puedo poner cualquier cosa
        //
        builder.Property(lib => lib.ID_Real)
          .HasColumnName("ID")
          .HasDefaultValueSql("newsequentialid()");

        builder.Property(lib => lib.Publicacion)
          .HasColumnName("Fecha_Publicacion")
          .HasDefaultValueSql("getdate()");
      }
      else
      {
        builder.Property(lib => lib.ID_Real)
          .HasColumnName("ID");

        //  en este caso...como Publicacion es nullable, si no pongo un valor por defecto, cualquier fecha nula me
        //  produciria un error de insert...
        //  Con HasDefaultValue, le aviso a EF que NO MANDE la columna y que despues la LEA desde la DB
        //  El valor que ponga por defecto ES IGNORADO
        //
        builder.Property(lib => lib.Publicacion)
          .HasColumnName("Fecha_Publicacion")
          .HasDefaultValue(DateTime.MinValue);
          //  .HasValueGenerator<ValueGeneratorDateTime>();
      }

      builder.Property(lib => lib.ID).HasColumnName("Clave_Origen");

      builder.Property(lib => lib.Publico).HasColumnName("Tipo_Publico");

      //  TODO_HECHO configurar precision decimal para eliminar el warning
      //  Con EFC 5 vamos a poder setear la precision 
      //  https://docs.microsoft.com/en-us/ef/core/what-is-new/ef-core-5.0/whatsnew#configure-database-precisionscale-in-model
      //
      builder
        .Property(lib => lib.Precio)
        .HasColumnType("numeric(10, 4)");

      //  setear el tipo de datos en SQL como float (que es la realidad) para que EF no tenga problemas
      //  en el mapeo e intente tomar un double
      //
      builder
        .Property(lib => lib.RatingPromedio)
        .HasColumnName("Promedio_Rating")
        .HasColumnType("float");

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
      builder.ToTable("Libros_Autores");

      builder.HasKey(la => new {la.ID_Libro, la.ID_Autor});

      builder
        .HasOne(la => la.Libro)
        .WithMany(lib => lib.LibroAutores)
        .HasForeignKey("ID_Libro");

      builder
        .HasOne(la => la.Autor)
        .WithMany(aut => aut.AutorLibros)
        .HasForeignKey(la=>la.ID_Autor);
    }
  }

  public class ConfigurarAutores : IEntityTypeConfiguration<Autor>
  {
    public void Configure(EntityTypeBuilder<Autor> builder)
    {
      //  builder.ToTable("Autores");

      builder.Property(aut => aut.Biografia).HasColumnName("Bio");
    }
  }


  public class ValueGeneratorDateTime : ValueGenerator<DateTime?>
  {
    public override DateTime? Next(EntityEntry entry)
    {
      return DateTime.MinValue;
    }

    public override bool GeneratesTemporaryValues { get; } = false;
  }
}
