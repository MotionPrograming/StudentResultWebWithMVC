using System.ComponentModel.DataAnnotations;

namespace StudentResultAppWithMVC.Models
{
    public class DepartmentViewModel
    {
        public int DepartmentId { get; set; }
        [Required]
        public string DepartmentName { get; set; }
    }
}
