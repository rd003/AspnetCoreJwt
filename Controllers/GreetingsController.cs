using Microsoft.AspNetCore.Mvc;

namespace AspnetCoreJwt.Controllers;

[Route("/api/greetings")]
[ApiController]
public class GreetingsController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("Hello everyone");

}