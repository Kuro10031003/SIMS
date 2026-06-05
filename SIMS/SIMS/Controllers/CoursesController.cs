using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SIMS.Data;
using SIMS.Models;

namespace SIMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CoursesController : Controller
    {
        private readonly AppDbContext _db;
        public CoursesController(AppDbContext db) { _db = db; }

        // READ
        public IActionResult Index()
        {
            var courses = _db.Courses.ToList();
            return View(courses);
        }

        // CREATE - GET
        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        // CREATE - POST
        [HttpPost]
        public IActionResult Create(Course course)
        {
            _db.Courses.Add(course);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        // EDIT - GET
        public IActionResult Edit(int id)
        {
            var course = _db.Courses.Find(id);
            if (course == null) return NotFound();
            LoadDropdowns();
            return View(course);
        }

        // EDIT - POST
        [HttpPost]
        public IActionResult Edit(Course course)
        {
            _db.Courses.Update(course);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        // DELETE - GET
        public IActionResult Delete(int id)
        {
            var course = _db.Courses.Find(id);
            if (course == null) return NotFound();
            return View(course);
        }

        // DELETE - POST
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var course = _db.Courses.Find(id);
            if (course != null)
            {
                _db.Courses.Remove(course);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // 辅助方法：把 Programme 和 Lecturer 列表准备好给下拉框用
        private void LoadDropdowns()
        {
            ViewBag.Programmes = new SelectList(_db.Programmes.ToList(), "ProgrammeID", "ProgrammeName");
            ViewBag.Lecturers = new SelectList(_db.Lecturers.ToList(), "LecturerID", "FullName");
        }
    }
}