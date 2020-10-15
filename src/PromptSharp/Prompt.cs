using System;
using System.Collections.Generic;
using PromptSharp.Forms;
using PromptSharp.Internal;
using PromptSharp.Validations;

namespace PromptSharp
{
  public static class Prompt
  {
    /// <summary>
    /// Muestra un menu de opciones a las cuales se les incorpora un indice numerico para identificarlas
    /// Retorna el indice elegido por el usuario
    /// </summary>
    /// <remarks>
    /// No deberia filtrar el menu...porque si tengo armada una estructura con opciones verticales haria un chiquero...
    /// Igual no permite opciones a la derecha (solo cambio de pagina)
    /// </remarks>
    /// <param name="header"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public static int Menu(string header, IEnumerable<string> items)
    {
      List<(int indice, string texto)> itemsReales = new List<(int, string)>();
      int idx = 0;

      foreach (string item in items)
      {
        itemsReales.Add((idx++, item));
      }

      var form = Select(header, itemsReales, null, null,
        tuple => $"[{tuple.indice:00}] .... {tuple.texto}");

      return form.indice;
    }

    public static T Input<T>(string message, object defaultValue = null,
        IList<Func<object, ValidationResult>> validators = null)
    {
      using var form = new Input<T>(message, defaultValue, validators);

      return form.Start();
    }

    public static string Password(string message, IList<Func<object, ValidationResult>> validators = null)
    {
      using var form = new Password(message, validators);

      return form.Start();
    }

    public static bool Confirm(string message, bool? defaultValue = null, string warning = null)
    {
      using var form = new Confirm(message, defaultValue, warning);

      return form.Start();
    }

    /// <summary>
    /// Permite seleccionar un valor a partir de un enum determindado
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message"></param>
    /// <param name="pageSize"></param>
    /// <param name="defaultValue"></param>
    /// <param name="valueSelector"></param>
    /// <returns></returns>
    public static T Select<T>(string message, int? pageSize = null, T? defaultValue = null,
        Func<T, string> valueSelector = null) where T : struct, Enum
    {
      var items = (T[])Enum.GetValues(typeof(T));

      using var form = new Select<T>(message, items, pageSize, defaultValue,
          valueSelector ?? (x => x.GetDisplayName()));

      return form.Start();
    }

    /// <summary>
    /// Retorna el elemento que se selecciona entre una enumeracion de los mismos
    /// Seria interesante mostrar que hay mas paginas de items...
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message"></param>
    /// <param name="items"></param>
    /// <param name="pageSize"></param>
    /// <param name="defaultValue"></param>
    /// <param name="valueSelector"></param>
    /// <returns></returns>
    public static T Select<T>(string message, IEnumerable<T> items, int? pageSize = null,
        object defaultValue = null, Func<T, string> valueSelector = null)
    {
      using var form = new Select<T>(message, items, pageSize, defaultValue,
          valueSelector ?? (x => x.ToString()));

      return form.Start();
    }

    public static IEnumerable<T> MultiSelect<T>(string message, int? pageSize = null, int minimum = 1,
        int maximum = -1, Func<T, string> valueSelector = null) where T : struct, Enum
    {
      var items = (T[])Enum.GetValues(typeof(T));

      using var form = new MultiSelect<T>(message, items, pageSize, minimum, maximum,
          valueSelector ?? (x => x.GetDisplayName()));

      return form.Start();
    }

    public static IEnumerable<T> MultiSelect<T>(string message, IEnumerable<T> items, int? pageSize = null,
        int minimum = 1, int maximum = -1, Func<T, string> valueSelector = null)
    {
      using var form = new MultiSelect<T>(message, items, pageSize, minimum, maximum,
          valueSelector ?? (x => x.ToString()));

      return form.Start();
    }

    public static class ColorSchema
    {
      public static ConsoleColor Answer { get; set; } = ConsoleColor.Cyan;
      public static ConsoleColor Select { get; set; } = ConsoleColor.Green;
      public static ConsoleColor DisabledOption { get; set; } = ConsoleColor.DarkCyan;
    }
  }
}
