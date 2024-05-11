using AspnetCoreJwt.Models;
using Microsoft.AspNetCore.Identity;

namespace AspnetCoreJwt.Repositories;
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
}

public interface IAuthService
{
    Task<(int, string)> Register(SignupModel signup, string role);
}