using System.Text.Json.Serialization;

namespace BlazorValidation.Client.Pages
{
  public static class DebuggingExtensions
  {
    public static string ToJson(this object obj)
      => JsonSerializer.ToString(obj, obj.GetType());
  }
}
