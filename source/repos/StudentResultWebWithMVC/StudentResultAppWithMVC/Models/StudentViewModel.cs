using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace StudentResultAppWithMVC.Models
{
    public class StudentViewModel
    {
        public int StudentId { get; set; }
        [Required]
        public string StudentName { get; set; }
        [Required]
        public int StudentAge { get; set; }
        public double TotalMark { get; set; }
        public double AverageMark { get; set; }
        public double CGPA { get; set; }
        public string? LetterGrade { get; set; }
        [Required(ErrorMessage = "Department is required")]
        public int DepartmentId { get; set; }
        public IEnumerable<SelectListItem>? Departments { get; set; }
    }
}
