using Microsoft.AspNetCore.Mvc;

namespace FleetMGMT.Api.Controllers;

public class HomeController : ControllerBase
{
  /// <summary>
  /// Status da aplicação e versão atual
  /// </summary>
  [HttpGet]
  [Route("")]
  public object Get() => new { app = "Fleet Management API", version = "Version 1.0" };
}
