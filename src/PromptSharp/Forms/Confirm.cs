using System;
using System.Resources;
using PromptSharp.Utiles;
using PromptSharp.Validations;

namespace PromptSharp.Forms
{
  internal class Confirm : FormBase<bool>
  {
    public Confirm(string message, bool? defaultValue, string warning)
    {
      _message = message;
      _defaultValue = defaultValue;
      _warning = warning;

      _yesOptions = RM.GetString("yesValidOptionsLower");
      _noOptions = RM.GetString("noValidOptionsLower");
    }

    private readonly string _message;
    private readonly bool? _defaultValue;

    private readonly string _warning;
    private readonly string _yesOptions;
    private readonly string _noOptions;

    protected override bool TryGetResult(out bool result)
    {
      var input = Renderer.ReadLine();

      if (string.IsNullOrEmpty(input))
      {
        if (_defaultValue != null)
        {
          result = _defaultValue.Value;

          return true;
        }

        Renderer.SetValidationResult(new ValidationResult(RM.GetString("valueRequired")));
      }
      else
      {
        //  var lowerInput = input.ToLower();
        //  if (lowerInput == "y" || lowerInput == "yes" || lowerInput == "s" || lowerInput == "si")
        if (input.In(_yesOptions))
        {
          result = true;

          return true;
        }

        //  if (lowerInput == "n" || lowerInput == "no")
        if (input.In(_noOptions))
        {
          result = false;

          return true;
        }

        Renderer.SetValidationResult(new ValidationResult(RM.GetString("valueInvalid")));
      }

      result = default;

      return false;
    }

    protected override void InputTemplate(FormRenderer formRenderer)
    {
      formRenderer.WriteMessage(_message);

      if (_defaultValue != null)
      {
        formRenderer.Write($"({(_defaultValue.Value ? "yes" : "no")}) ");
      }
      else
      {
        formRenderer.Write("(y/N) ");
      }

      if (_warning != null)
      {
        //  TODO ver el tema de que cuando borra el template (o sea todo lo que escribio) usa la pos del cursor actual
        //  entonces si agregue lineas, aparecen en el total pero el cursor es engañoso deberia guardar la pos
        //  de origen del template para saber exactamente desde donde borrar!!
        //
        //formRenderer.SaveCursor();
        formRenderer.WriteLine();
        formRenderer.Write(_warning, ConsoleColor.Red);
        //formRenderer.RestoreCursor();
      }
    }

    protected override void FinishTemplate(FormRenderer formRenderer, bool result)
    {
      formRenderer.WriteFinishMessage(_message);
      formRenderer.Write(result ? "Yes" : "No", Prompt.ColorSchema.Answer);
    }
  }
}
