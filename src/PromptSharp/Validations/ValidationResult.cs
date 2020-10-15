namespace PromptSharp.Validations
{
  public class ValidationResult
  {
    public ValidationResult(string errorMessage)
    {
      ErrorMessage = errorMessage;
    }

    public string ErrorMessage { get; }
  }
}
