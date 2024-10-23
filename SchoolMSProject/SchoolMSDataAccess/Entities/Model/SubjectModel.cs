using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolMSDataAccess.Entities.Model
{
    public class SubjectModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Class is required")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Class must be Numeric.")]
        public string Class { get; set; }

        [Required(ErrorMessage = "Language is required")]
        [StringLength(100, ErrorMessage = "Language cannot be longer than 100 characters.")]
        public string Language { get; set; }

        public int TeacherId { get; set; }
        public IEnumerable<TeacherModel> teachers { get; set; }
    }
}
