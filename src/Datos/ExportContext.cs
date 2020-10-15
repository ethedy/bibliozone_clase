#define USE_ADDDBCONTEXT

using System;
using System.Collections.Generic;
using System.Text;
using Entidades.Articulos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Datos
{
  public class ExportContext : DbContext
  {
    private readonly IConfiguration _config;

    private readonly ILogger<ExportContext> _logger;

    private readonly ILoggerFactory _loggerFactory;

    //  public DbSet<Libro> Libros { get; set; }

    public DbSet<Autor> Autores { get; set; }

#if USE_ADDDBCONTEXT

    public ExportContext(DbContextOptions<ExportContext> options, IConfiguration config, ILogger<ExportContext> logger) : base(options)
    {
      //  TODO_HECHO cambiar nombre del contexto para adecuarlo a la funcion
      //  TODO usar nombre del contexto para obtener la cadena de conexion

      _config = config;
      _logger = logger;
      //  _logger = this.GetService<ILogger<BZoneContext>>();

      _logger.LogWarning("Creado contexto {contexto} desde AddDbContext<T>", nameof(ExportContext));
    }

#else

    /// <summary>
    /// ctor necesario para usar DI tradicional
    /// </summary>
    /// <param name="loggerFactory"></param>
    /// <param name="config"></param>
    public BZoneContext(ILoggerFactory loggerFactory, IConfiguration config)
    {
      _config = config;
      _loggerFactory = loggerFactory;

      _logger = _loggerFactory.CreateLogger<BZoneContext>();

      _logger.LogWarning("Creado contexto {contexto} desde ctor normal", nameof(BZoneContext));
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      _logger.LogWarning("Empezando la configuracion del contexto {contexto}", nameof(BZoneContext));

      optionsBuilder
        .UseSqlServer(_config.GetConnectionString("curso"))
        .EnableDetailedErrors()
        .EnableSensitiveDataLogging()
        .UseLoggerFactory(_loggerFactory);    //  TODO Observar que pasa si nos olvidamos la factory!!

      base.OnConfiguring(optionsBuilder);
    }

#endif

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      _logger.LogWarning("Empezando la creacion del modelo para el contexto {contexto}", nameof(ExportContext));

      modelBuilder.ApplyConfiguration(new ConfigurarLibros(_config));
      modelBuilder.ApplyConfiguration(new ConfigurarLibrosAutores());
      modelBuilder.ApplyConfiguration(new ConfigurarAutores());

      base.OnModelCreating(modelBuilder);
    }

    public void Debug()
    {
      if (ChangeTracker.HasChanges())
      {
        Console.WriteLine("Status del Contexto");

        foreach (var item in ChangeTracker.Entries())
        {
          Console.WriteLine($"{item.Entity.GetType()} {item.State}");
          if (item.State == EntityState.Modified)
          {
            foreach (var prop in item.Properties)
            {
              Console.WriteLine($"==> {prop.Metadata.Name} -- {(prop.IsModified ? "Modificada":"No modificada")}");
            }
          }
        }
      }
    }
  }
}
