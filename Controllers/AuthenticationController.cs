using AspnetCoreJwt.Models;
using AspnetCoreJwt.Services;
using Microsoft.AspNetCore.Mvc;

namespace AspnetCoreJwt.Controllers;

[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthenticationController> _logger;
    public AuthenticationController(IAuthService authService, ILogger<AuthenticationController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("/register")]
    public async Task<IActionResult> Register(SignupModel model)
    {
        try
        {
            (int status, string message) = await _authService.Register(model, UserRoles.User);
            // For Admin
            // (int status, string message) = await _authService.Register(model, UserRoles.Admin);
            if (status == 1)
            {
                return Ok(new { model.Email, model.Name });
            }
            else
            {
                return BadRequest(message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
        }
    }

    [HttpPost("/login")]
    public async Task<IActionResult> Login(LoginModel model)
    {
        try
        {
            (int status, string message) = await _authService.Login(model);
            if (status == 1)
            {
                return Ok(new LoginResponse(AccessToken: message));
            }
            else
            {
                return BadRequest(message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
        }
    }

}