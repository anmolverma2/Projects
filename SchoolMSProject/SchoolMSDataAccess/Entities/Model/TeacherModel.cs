using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolMSDataAccess.Entities.Model
{
    public class TeacherModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        public string ImagePath { get; set; }

        [Required(ErrorMessage = "Age is required")]
        [Range(1, 50, ErrorMessage = "Age must be between 1 and 50.")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Sex is required")]
        public string Sex { get; set; }

        [Required(ErrorMessage = "Image is required")]
        public IFormFile formFile { get; set; }

    }
}
