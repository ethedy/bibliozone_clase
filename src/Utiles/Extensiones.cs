using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utiles
{
  public static class Extensiones
  {
    /// <summary>
    /// Permite chequear si una cadena es una de las especificadas en la coleccion enumerable
    /// </summary>
    /// <param name="src"></param>
    /// <param name="testList"></param>
    /// <returns></returns>
    public static bool In(this string src, IEnumerable<string> testList)
    {
      return testList.Contains(src.Trim(), StringComparer.InvariantCultureIgnoreCase);
    }

    public static bool In(this string src, string test, char sep = ';')
    {
      return src.In(test.Split(sep));
    }

    public static bool In<T>(this T src, IEnumerable<T> testList)
    {
      return testList.Contains(src);
    }

    public static bool In<T>(this T src, params T[] items)
    {
      return items.Contains(src);
    }

    public static string NullIfEmpty(this string src)
    {
      return string.IsNullOrWhiteSpace(src) ? null : src;
    }

    public static string ActionIfEmpty(this string src, Action accion)
    {
      if (string.IsNullOrWhiteSpace(src))
      {
        accion();
        return null;
      }
      else
        return src;
    }

  }
}
