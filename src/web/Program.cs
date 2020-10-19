using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace web
{
  public class Program
  {
    public static void Main(string[] args)
    {
      CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host
        .CreateDefaultBuilder(args)
        /*

        //  Paquete: Serilog.Extensions.Logging
        //
        .ConfigureLogging(builder => builder.AddSerilog(
          new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console()
            .WriteTo.File("web.log")
            .WriteTo.Seq("http://localhost:5341/")
            .CreateLogger()))
        */
        /*

        //  Paquete: Serilog.Extensions.Hosting
        //
        .UseSerilog((ctx, cfg) =>
        {
          cfg
            .MinimumLevel.Verbose()
            .WriteTo.Console()
            .WriteTo.File("web.log")
            .WriteTo.Seq("http://localhost:5341/");
        })
        */
        .ConfigureWebHostDefaults(webBuilder =>
        {
          webBuilder
            .UseStartup<Startup>()

            //  Paquete: Serilog.AspNetCore
            //
            .UseSerilog((ctx, cfg) =>
            {
              cfg
                .MinimumLevel.Verbose()
                //  .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Debug)
                .WriteTo.Console()
                .WriteTo.File("web.log")
                .WriteTo.Seq("http://localhost:5341/");
            });
        });

  }
}
