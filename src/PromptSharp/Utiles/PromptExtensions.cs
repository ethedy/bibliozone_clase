using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PromptSharp.Utiles
{
  public static class PromptExtensions
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
  }
}
