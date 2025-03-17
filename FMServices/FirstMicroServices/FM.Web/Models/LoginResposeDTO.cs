using System.ComponentModel.DataAnnotations;

namespace FM.Web.Models
{
    public class LoginResposeDTO
    {
        [Required]
        public UserDTO User { get; set; }
        [Required]
        public string Token { get; set; }
    }
}
