using System.ComponentModel.DataAnnotations;

namespace PracticeAPI.Model
{
    public class RolePrivilageDTO
    {
        public int Id { get; set; }
        [Required]
        public string RolePrivilegeName { get; set; }
        public string Description { get; set; }
        public int RoleId { get; set; }
        [Required]
        public bool IsActive { get; set; }
    }
}
