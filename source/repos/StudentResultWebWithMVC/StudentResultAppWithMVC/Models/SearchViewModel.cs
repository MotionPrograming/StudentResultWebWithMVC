using Microsoft.AspNetCore.Mvc.Rendering;
using StudentResultAppWithMVC.Models.Entity;

namespace StudentResultAppWithMVC.Models
{
    public class SearchViewModel
    {
        public string? SearchQuery { get; set; }
        public int? DepartmentId { get; set; }
        public int? Age { get; set; }
        public string? LetterGrade { get; set; }
        public IEnumerable<Student> Students { get; set; } = new List<Student>();
        public IEnumerable<SelectListItem>? Departments { get; set; }
    }
}
