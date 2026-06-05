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

        // 查看某门课的学生名单
        public IActionResult CourseStudents(int id)
        {
            var lecturer = GetCurrentLecturer();
            if (lecturer == null) return Content("Lecturer profile not found.");

            // 确认这门课确实是这个讲师教的（安全检查）
            var course = _db.Courses.FirstOrDefault(c => c.CourseID == id && c.LecturerID == lecturer.LecturerID);
            if (course == null) return Content("Course not found or not yours.");

            // 找出选了这门课的学生
            var enrolments = _db.Enrolments.Where(e => e.CourseID == id).ToList();
            var studentIds = enrolments.Select(e => e.StudentID).ToList();
            var students = _db.Students.Where(s => studentIds.Contains(s.StudentID)).ToList();

            ViewBag.CourseName = course.CourseName;
            ViewBag.CourseCode = course.CourseCode;
            return View(students);
        }
    }
}