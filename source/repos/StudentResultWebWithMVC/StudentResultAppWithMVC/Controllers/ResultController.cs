using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentResultAppWithMVC.Data;
using StudentResultAppWithMVC.Models;
using StudentResultAppWithMVC.Models.Entity;

namespace StudentResultAppWithMVC.Controllers
{
    public class ResultController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResultController(ApplicationDbContext context)
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
            var results = await _context.results
                .Include(r => r.student)
                .ToListAsync();
            return View(results);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var studentsWithResults = await _context.results
                .Select(r => r.StudentId)
                .ToListAsync();

            var availableStudents = await _context.students
                .Where(s => !studentsWithResults.Contains(s.StudentId))
                .ToListAsync();

            var model = new ResultViewModel
            {
                Students = availableStudents.Select(s => new SelectListItem
                {
                    Value = s.StudentId.ToString(),
                    Text = s.StudentName
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ResultViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Students = (await _context.students.ToListAsync())
                    .Select(s => new SelectListItem
                    {
                        Value = s.StudentId.ToString(),
                        Text = s.StudentName
                    }).ToList();
                return View(model);
            }

            var student = await _context.students.FindAsync(model.StudentId);
            if (student == null)
            {
                ModelState.AddModelError("StudentId", "Selected student not found");
                model.Students = (await _context.students.ToListAsync())
                    .Select(s => new SelectListItem
                    {
                        Value = s.StudentId.ToString(),
                        Text = s.StudentName
                    }).ToList();
                return View(model);
            }

            var existingResult = await _context.results
                .FirstOrDefaultAsync(r => r.StudentId == model.StudentId);

            if (existingResult != null)
            {
                existingResult.PhysicsMark = model.PhysicsMark;
                existingResult.ChemistryMark = model.ChemistryMark;
                existingResult.MathMark = model.MathMark;
                _context.results.Update(existingResult);
            }
            else
            {
                var result = new Result
                {
                    StudentId = model.StudentId,
                    PhysicsMark = model.PhysicsMark,
                    ChemistryMark = model.ChemistryMark,
                    MathMark = model.MathMark
                };
                _context.results.Add(result);
            }

            await _context.SaveChangesAsync();
            await CalculateAndUpdateMarks(model.StudentId);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _context.results.FindAsync(id);
            if (result == null)
                return NotFound();

            var model = new ResultViewModel
            {
                ResultId = result.ResultId,
                PhysicsMark = result.PhysicsMark,
                ChemistryMark = result.ChemistryMark,
                MathMark = result.MathMark,
                StudentId = result.StudentId
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ResultViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existingResult = await _context.results.FindAsync(model.ResultId);
            if (existingResult == null)
                return NotFound();

            // Verify that the StudentId hasn't been tampered with
            if (existingResult.StudentId != model.StudentId)
                return BadRequest("Invalid student for this result");

            existingResult.PhysicsMark = model.PhysicsMark;
            existingResult.ChemistryMark = model.ChemistryMark;
            existingResult.MathMark = model.MathMark;

            _context.results.Update(existingResult);
            await _context.SaveChangesAsync();
            await CalculateAndUpdateMarks(model.StudentId);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _context.results
                .Include(r => r.student)
                .FirstOrDefaultAsync(r => r.ResultId == id);

            if (result == null)
                return NotFound();

            return View(result);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _context.results.FindAsync(id);
            if (result == null)
                return NotFound();

            var studentId = result.StudentId;
            _context.results.Remove(result);
            await _context.SaveChangesAsync();

            // Reset student marks when result is deleted
            var student = await _context.students.FindAsync(studentId);
            if (student != null)
            {
                student.TotalMark = 0;
                student.AverageMark = 0;
                student.CGPA = 0;
                student.LetterGrade = null;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
