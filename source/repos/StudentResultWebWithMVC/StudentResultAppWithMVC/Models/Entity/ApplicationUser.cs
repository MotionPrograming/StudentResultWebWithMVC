using Microsoft.AspNetCore.Identity;

namespace StudentResultAppWithMVC.Models.Entity
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
