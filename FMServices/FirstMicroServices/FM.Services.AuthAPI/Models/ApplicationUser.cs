using Microsoft.AspNetCore.Identity;

namespace FM.Services.AuthAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
