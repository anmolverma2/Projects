using System.ComponentModel.DataAnnotations;

namespace FM.Web.Models
{
    public class RegistrationRequestDTO
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string? RoleName { get; set; }
    }
}
