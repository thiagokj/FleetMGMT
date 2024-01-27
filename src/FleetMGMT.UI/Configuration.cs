namespace FleetMGMT.UI
{
  public static class Configuration
  {
    public static ApiConfiguration Api { get; set; } = new();

    public class ApiConfiguration
    {
      public string BaseUrl { get; set; } = string.Empty;
    }
  }
}
