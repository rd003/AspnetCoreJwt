using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AspnetCoreJwt.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace AspnetCoreJwt.Services;
public class AuthService : IAuthService
{
    readonly IConfiguration _configuration;
    readonly UserManager<ApplicationUser> _userManager;
    readonly RoleManager<IdentityRole> _roleManager;

    public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    public async Task<(int, string)> Register(SignupModel signup, string role)
    {
        // check user exists or not
        var user = await _userManager.FindByEmailAsync(signup.Email);
        if (user != null)
        {
            return (0, "User is already exists");
        }
        ApplicationUser appUser = new()
        {
            Name = signup.Name,
            UserName = signup.Email,
            Email = signup.Email,
            SecurityStamp = Guid.NewGuid().ToString()
        };
        var createdUserResult = await _userManager.CreateAsync(appUser, signup.Password);
        if (!createdUserResult.Succeeded)
        {
            return (0, "User creation failed! Please check user details and try again.");
        }
        // create role if does not exists
        bool isRoleExists = await _roleManager.RoleExistsAsync(role);
        if (!isRoleExists)
        {
            await _roleManager.CreateAsync(new IdentityRole(role));
        }
        // add user's role
        await _userManager.AddToRoleAsync(appUser, role);
        return (1, "User created successfully!");
    }

    public async Task<(int, string)> Login(LoginModel model)
    {
        // find user by email
        ApplicationUser? user = await _userManager.FindByEmailAsync(model.Username);
        if (user == null)
        {
            return (0, "Invalid Email");
        }
        // match password
        bool isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!isPasswordValid)
        {
            return (0, "Invalid Password");
        }

        // get user's roles
        IList<string> userRoles = await _userManager.GetRolesAsync(user);

        // create claims
        List<Claim> claims = [
            new Claim(ClaimTypes.Name,user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
        ];
        foreach (string role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        string token = GenerateToken(claims);
        // generate token
        return (1, token);
    }

    private string GenerateToken(IEnumerable<Claim> claims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _configuration["JWT:ValidIssuer"],
            Audience = _configuration["JWT:ValidAudience"],
            Expires = DateTime.UtcNow.AddMinutes(2),
            SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(claims)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

public interface IAuthService
{
    Task<(int, string)> Register(SignupModel signup, string role);
    Task<(int, string)> Login(LoginModel model);
}