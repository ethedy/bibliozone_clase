using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using System.Text;

namespace PromptSharp.Utiles
{
  internal static class RM
  {
    private static ResourceManager _rm =
      new ResourceManager("PromptSharp.Recursos.Mensajes", Assembly.GetAssembly(typeof(RM)));

    public static string GetString(string clave)
    {
      return _rm.GetString(clave);
    }
  }
}
