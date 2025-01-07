using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracticeAPI.Model
{
    public class CollegeStudent
    {
      //  [Key]
      //  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Name { get; set; }        
        public int Age { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public DateTime? DOB { get; set; }
        public int? DepartmentId { get; set; }
        public virtual Department? department { get; set; }
        
    }
}
