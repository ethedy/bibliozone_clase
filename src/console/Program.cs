#define USE_ADDDBCONTEXT

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Datos;
using Entidades.Articulos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Servicios;

namespace console
{
  class Program
  {
    static void Main(string[] args)
    {
      var builder = Host.CreateDefaultBuilder();

      builder.ConfigureAppConfiguration(option =>
        {
          option.SetBasePath(Directory.GetCurrentDirectory());
          option.AddJsonFile("consola.json");
          option.AddCommandLine(args);
        })
        .ConfigureServices((ctx, serv) => 
        {
          serv.AddScoped<ServiciosImportacion>();
          serv.AddScoped<Aplicacion>();
          serv.AddScoped<ServiciosExportacion>();

#if USE_ADDDBCONTEXT
          serv.AddDbContext<BZoneContext>(build =>
          {
            build.UseSqlServer(ctx.Configuration.GetConnectionString("curso"));
            build.EnableDetailedErrors();
            build.EnableSensitiveDataLogging();
          });
#else
          serv.AddScoped<BZoneContext>();
#endif
        })
        .ConfigureLogging(logBuilder =>
          {
            logBuilder.ClearProviders();

            logBuilder.AddSerilog(new LoggerConfiguration()
              .MinimumLevel.Verbose()
              .WriteTo.Console()
              .WriteTo.File("console.log")
              .CreateLogger());
          }
        );

      var host = builder.Build();

      Aplicacion app = host.Services.GetService<Aplicacion>();

      app.Run();


      Console.WriteLine("Presionar ENTER");
      Console.ReadLine();
    }
  }
}
