using Microsoft.AspNetCore.Identity;

namespace AspnetCoreJwt.Models;

public class ApplicationUser : IdentityUser
{
    public required string Name { get; set; }
}