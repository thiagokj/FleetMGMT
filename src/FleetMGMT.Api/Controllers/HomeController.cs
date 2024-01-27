using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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
