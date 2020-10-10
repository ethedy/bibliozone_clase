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
  public class BZoneContext : DbContext
  {
    private readonly IConfiguration _config;

    private readonly ILogger<BZoneContext> _logger;

    private readonly ILoggerFactory _loggerFactory;

    //  public DbSet<Libro> Libros { get; set; }

#if USE_ADDDBCONTEXT

    public BZoneContext(DbContextOptions<BZoneContext> options, IConfiguration config, ILogger<BZoneContext> logger) : base(options)
    {
      //  TODO cambiar nombre del contexto para adecuarlo a la funcion
      //  TODO usar nombre del contexto para obtener la cadena de conexion

      _config = config;
      _logger = logger;
      //  _logger = this.GetService<ILogger<BZoneContext>>();

      _logger.LogWarning("Creado contexto {contexto} desde AddDbContext<T>", nameof(BZoneContext));
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
      _logger.LogWarning("Empezando la creacion del modelo para el contexto {contexto}", nameof(BZoneContext));

      modelBuilder.ApplyConfiguration(new ConfigurarLibros());
      modelBuilder.ApplyConfiguration(new ConfigurarLibrosAutores());

      base.OnModelCreating(modelBuilder);
    }
  }
}
