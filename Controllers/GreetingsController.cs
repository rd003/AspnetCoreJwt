using AspnetCoreJwt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspnetCoreJwt.Controllers;

[Route("/api/greetings")]
[Authorize(Roles = UserRoles.Admin)]
[ApiController]
public class GreetingsController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("Hello everyone");

}