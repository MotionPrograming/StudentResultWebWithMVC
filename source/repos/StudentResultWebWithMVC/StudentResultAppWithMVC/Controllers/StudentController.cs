using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentResultAppWithMVC.Data;
using StudentResultAppWithMVC.Models;
using StudentResultAppWithMVC.Models.Entity;

namespace StudentResultAppWithMVC.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task CalculateAndUpdateMarks(int studentId)
        {
            var student = await _context.students.FindAsync(studentId);
            if (student == null)
                return;

            var result = await _context.results
                .FirstOrDefaultAsync(r => r.StudentId == studentId);

            if (result != null)
            {
                student.TotalMark = result.PhysicsMark + result.ChemistryMark + result.MathMark;

                student.AverageMark = student.TotalMark / 3;
                if (student.AverageMark > 80)
                {
                    student.LetterGrade = "A+";
                    student.CGPA = 4.00;
                }
                else if (student.AverageMark > 70)
                {
                    student.LetterGrade = "A";
                    student.CGPA = 3.75;
                }
                else if (student.AverageMark > 65)
                {
                    student.LetterGrade = "A-";
                    student.CGPA = 3.50;
                }
                else if (student.AverageMark > 60)
                {
                    student.LetterGrade = "B";
                    student.CGPA = 3.00;
                }
                else if (student.AverageMark > 50)
                {
                    student.LetterGrade = "C";
                    student.CGPA = 2.00;
                }
                else if (student.AverageMark > 40)
                {
                    student.LetterGrade = "D";
                    student.CGPA = 1.00;
                }
                else
                {
                    student.LetterGrade = "F";
                    student.CGPA = 0.00;
                }

                await _context.SaveChangesAsync();
            }
        }
        public async Task<IActionResult> Index()
        {
            var students = await _context.students
                .Include(s => s.department)
                .ToListAsync();

            foreach (var student in students)
            {
                await CalculateAndUpdateMarks(student.StudentId);
            }

            return View(students);
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var model = new StudentViewModel
            {
                Departments = await _context.departments
                    .Select(d => new SelectListItem
                    {
                        Value = d.DepartmentId.ToString(),
                        Text = d.DepartmentName
                    })
                    .ToListAsync()
            };

            return View(model);
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Departments = await _context.departments
                    .Select(d => new SelectListItem
                    {
                        Value = d.DepartmentId.ToString(),
                        Text = d.DepartmentName
                    })
                    .ToListAsync();

                return View(model);
            }

            var student = new Student
            {
                StudentName = model.StudentName,
                StudentAge = model.StudentAge,
                DepartmentId = model.DepartmentId
            };

            _context.students.Add(student);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _context.students.FindAsync(id);

            if (student == null)
                return NotFound();

            var model = new StudentViewModel
            {
                StudentId = student.StudentId,
                StudentName = student.StudentName,
                StudentAge = student.StudentAge,
                DepartmentId = student.DepartmentId,

                Departments = await _context.departments
                    .Select(d => new SelectListItem
                    {
                        Value = d.DepartmentId.ToString(),
                        Text = d.DepartmentName
                    })
                    .ToListAsync()
            };

            return View(model);
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Departments = await _context.departments
                    .Select(d => new SelectListItem
                    {
                        Value = d.DepartmentId.ToString(),
                        Text = d.DepartmentName
                    })
                    .ToListAsync();

                return View(model);
            }

            var student = await _context.students.FindAsync(model.StudentId);

            if (student == null)
                return NotFound();

            student.StudentName = model.StudentName;
            student.StudentAge = model.StudentAge;
            student.DepartmentId = model.DepartmentId;

            await _context.SaveChangesAsync();
            await CalculateAndUpdateMarks(model.StudentId);

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _context.students
                .Include(s => s.department)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (model == null)
                return NotFound();

            return View(model);
        }
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.students.FindAsync(id);
            if (model == null) return NotFound();

            _context.students.Remove(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


    }
}
