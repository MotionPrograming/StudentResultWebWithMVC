using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentResultAppWithMVC.Data;
using StudentResultAppWithMVC.Models;
using StudentResultAppWithMVC.Models.Entity;

namespace StudentResultAppWithMVC.Controllers
{
    [Authorize]
    public class DepartmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DepartmentController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var Student = await _context.departments.ToListAsync();
            return View(Student);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartmentViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var department = new Department()
            {
                DepartmentName = model.DepartmentName
            };

            _context.departments.Add(department);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.departments.FindAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Department model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.departments.Update(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _context.departments.FindAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.departments.FindAsync(id);
            if (model == null) return NotFound();

            _context.departments.Remove(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
