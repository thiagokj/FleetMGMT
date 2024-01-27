using FleetMGMT.UI.Contexts.VehicleContext.UseCases.CRUD;

namespace FleetMGMT.UI.Extensions;
public static class BuilderExtensions
{
  public static void AddConfiguration(this WebApplicationBuilder builder)
  {
    Configuration.Api.BaseUrl =
               builder.Configuration.GetSection("Secrets")
               .GetValue<string>("ApiBaseUrl") ?? string.Empty;
  }

  public static void AddServices(this WebApplicationBuilder builder)
  {
    builder.Services.AddControllersWithViews();
    builder.Services.AddHttpClient<VehicleService, VehicleService>(
    client =>
    {
      client.BaseAddress = new Uri(Configuration.Api.BaseUrl);
    });
  }
}