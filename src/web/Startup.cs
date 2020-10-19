using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Context;
using Servicios;

namespace web
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllersWithViews();

      services.AddDbContext<ExportContext>(build =>
      {
        build.UseSqlServer(Configuration.GetConnectionString("curso"));
        build.EnableDetailedErrors();
        build.EnableSensitiveDataLogging();
      });
      services.AddScoped<ServiciosExportacion>();
    }

    //  Analizar la URL para obtener los fragmentos {controller} y {action}
    //  http://midominio/home/test
    //
    //  This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
    {
      app.UseSerilogRequestLogging();

      if (env.IsDevelopment())
      {
        //  app.UseDeveloperExceptionPage();
        //  app.UseExceptionHandler("/Home/Error");

        app.UseWhen(ctx => ctx.Request.Path.Value?.Contains("/home/index") ?? false,
          cfg =>
          {
            cfg.UseExceptionHandler(bldr => bldr.Run(async context =>
              {
                //  si podemos capturar que excepcion produjo la entrada a este handler, la guardamos en esta variable
                //
                var excepcion = context.Features.Get<IExceptionHandlerPathFeature>();

                if (excepcion != null)
                {
                  logger.LogError(excepcion.Error, "EXCEPCION!! {PathOriginal} {PathExcepcion}", excepcion.Path,
                    context.Request.Path.Value);

                  context.Response.Headers["X-Powered-By"] = "Middleware NET Core API by POLO TECNOLOGICO";
                  context.Response.ContentType = "text/plain";  
                  await context.Response.WriteAsync($"{excepcion.Error.Message} - {excepcion.Path}");
                }
              }));
          }
        );
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
      }

      app.UseStaticFiles();

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
          name: "default",
          pattern: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}
