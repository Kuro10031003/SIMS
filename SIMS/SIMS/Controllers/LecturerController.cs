using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS.Data;
using SIMS.Models;

namespace SIMS.Controllers
{
    [Authorize(Roles = "Lecturer")]
    public class LecturerController : Controller
    {
        private readonly AppDbContext _db;
        public LecturerController(AppDbContext db) { _db = db; }

        // 辅助方法：拿到当前登录的讲师
        private Lecturer? GetCurrentLecturer()
        {
            var username = User.Identity?.Name;
            var user = _db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return null;
            return _db.Lecturers.FirstOrDefault(l => l.UserID == user.UserID);
        }

        // Dashboard
        public IActionResult Index()
        {
            var lecturer = GetCurrentLecturer();
            if (lecturer == null) return Content("Lecturer profile not found.");

            // 统计自己教的课、学生数
            var myCourses = _db.Courses.Where(c => c.LecturerID == lecturer.LecturerID).ToList();
            var courseIds = myCourses.Select(c => c.CourseID).ToList();
            var studentCount = _db.Enrolments.Where(e => courseIds.Contains(e.CourseID)).Count();

            ViewBag.LecturerName = lecturer.FullName;
            ViewBag.CourseCount = myCourses.Count;
            ViewBag.StudentCount = studentCount;
            return View();
        }

        // 查看自己任教的课程
        public IActionResult Courses()
        {
            var lecturer = GetCurrentLecturer();
            if (lecturer == null) return Content("Lecturer profile not found.");

            var myCourses = _db.Courses.Where(c => c.LecturerID == lecturer.LecturerID).ToList();
            return View(myCourses);
        }
    }
}