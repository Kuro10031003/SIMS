using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS.Data;

namespace SIMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;
        public AdminController(AppDbContext db) { _db = db; }

        public IActionResult Index()
        {
            ViewBag.TotalStudents = _db.Students.Count();
            ViewBag.TotalLecturers = _db.Lecturers.Count();
            ViewBag.TotalCourses = _db.Courses.Count();
            ViewBag.TotalProgrammes = _db.Programmes.Count();
            return View();
        }
    }
}