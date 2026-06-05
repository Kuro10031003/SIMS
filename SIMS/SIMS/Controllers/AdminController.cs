using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SIMS.Data;
using SIMS.Models;

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

        // REGISTER STUDENT - GET
        public IActionResult RegisterStudent()
        {
            ViewBag.Programmes = new SelectList(_db.Programmes.ToList(), "ProgrammeID", "ProgrammeName");
            return View();
        }

        // REGISTER STUDENT - POST
        [HttpPost]
        public IActionResult RegisterStudent(string username, string email, string password, string fullName, int programmeID, string? phone)
        {
            // 1. 先建 Users 账号
            var hasher = new PasswordHasher<User>();
            var user = new User
            {
                Username = username,
                Email = email,
                Role = "Student",
                IsActive = true,
                CreatedDate = DateTime.Now
            };
            user.PasswordHash = hasher.HashPassword(user, password);
            _db.Users.Add(user);
            _db.SaveChanges();   // 保存后 user.UserID 会自动生成

            // 2. 再建 Students 资料，关联刚建的 UserID
            var student = new Student
            {
                UserID = user.UserID,
                ProgrammeID = programmeID,
                FullName = fullName,
                Phone = phone,
                EnrollDate = DateTime.Now,
                Status = "Active"
            };
            _db.Students.Add(student);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}