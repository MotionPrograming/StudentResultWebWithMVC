using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentResultAppWithMVC.Data;
using StudentResultAppWithMVC.Models;

namespace StudentResultAppWithMVC.Controllers
{
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? searchQuery, int? departmentId, int? age, string? letterGrade)
        {
            var model = new SearchViewModel
            {
                SearchQuery = searchQuery,
                DepartmentId = departmentId,
                Age = age,
                LetterGrade = letterGrade,
                Departments = await _context.departments
                    .Select(d => new SelectListItem
                    {
                        Value = d.DepartmentId.ToString(),
                        Text = d.DepartmentName
                    })
                    .ToListAsync()
            };

            var query = _context.students
                .Include(s => s.department)
                .Include(s => s.result)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(s =>
                    s.StudentName.Contains(searchQuery) ||
                    s.StudentId.ToString().Contains(searchQuery));
            }

            if (departmentId.HasValue && departmentId.Value > 0)
            {
                query = query.Where(s => s.DepartmentId == departmentId.Value);
            }

            if (age.HasValue && age.Value > 0)
            {
                query = query.Where(s => s.StudentAge == age.Value);
            }

            if (!string.IsNullOrWhiteSpace(letterGrade))
            {
                query = query.Where(s => s.LetterGrade == letterGrade);
            }

            model.Students = await query
                .OrderBy(s => s.StudentId)
                .ToListAsync();

            return View(model);
        }
    }
}
