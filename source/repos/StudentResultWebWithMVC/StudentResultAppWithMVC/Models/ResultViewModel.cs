using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace StudentResultAppWithMVC.Models
{
    public class ResultViewModel
    {
        public int ResultId { get; set; }

        [Required(ErrorMessage = "Physics marks are required")]
        [Range(0, 100, ErrorMessage = "Marks must be between 0 and 100")]
        public double PhysicsMark { get; set; }

        [Required(ErrorMessage = "Chemistry marks are required")]
        [Range(0, 100, ErrorMessage = "Marks must be between 0 and 100")]
        public double ChemistryMark { get; set; }

        [Required(ErrorMessage = "Math marks are required")]
        [Range(0, 100, ErrorMessage = "Marks must be between 0 and 100")]
        public double MathMark { get; set; }

        public int StudentId { get; set; }
        public IEnumerable<SelectListItem>? Students { get; set; }
    }
}
