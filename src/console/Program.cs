using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Entidades.Articulos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Servicios;

namespace console
{
  class Program
  {
    static void Main(string[] args)
    {
      Aplicacion app = new Aplicacion(new ServiciosImportacion());

      app.Run();

      Console.WriteLine("Presionar ENTER");
      Console.ReadLine();
    }
  }
}
