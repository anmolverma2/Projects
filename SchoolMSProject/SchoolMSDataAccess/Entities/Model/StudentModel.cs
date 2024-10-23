using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolMSDataAccess.Entities.Model
{
    public class StudentModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Age is required")]
        [Range(1, 50, ErrorMessage = "Age must be between 1 and 50.")]
        public int Age { get; set; }
        public string Image { get; set; }

        [Required(ErrorMessage = "Class is Required")]

        [Range(1, 12, ErrorMessage = "Class must be between 1 and 12.")]
        public string Class { get; set; }

        [Required(ErrorMessage = "RollNumber is Required")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Roll Number must be numeric")]
        public string RollNumber { get; set; }

        [Required(ErrorMessage = "Please select an image")]
        public IFormFile formFile { get; set; }
        public int SubjectId { get; set; }
        public IEnumerable<StudentModel> studentModels { get; set; }
        public IEnumerable<SubjectModel> subjects { get; set; }
    }
}
