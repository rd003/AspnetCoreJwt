using AspnetCoreJwt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspnetCoreJwt.Controllers;

[Route("/api/greetings")]
[Authorize(Roles = UserRoles.Admin)]
[ApiController]
public class GreetingsController : ControllerBase
{
    // GET: api/greetings
    [HttpGet]
    public IActionResult Get() => Ok("Hello everyone");

    // GET: api/greetings/public
    [HttpGet("public")]
    [AllowAnonymous]
    public IActionResult PublicMethod() => Ok("Hello everyone");

    // POST: api/greetings
    [HttpPost]
    public IActionResult Post() => Ok("Posted successfully");

}