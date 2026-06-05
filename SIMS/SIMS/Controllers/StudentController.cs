using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS.Data;
using SIMS.Models;

namespace SIMS.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly AppDbContext _db;
        public StudentController(AppDbContext db) { _db = db; }

        // 拿当前登录的学生（跟讲师那个一样的套路）
        private Student? GetCurrentStudent()
        {
            var username = User.Identity?.Name;
            var user = _db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return null;
            return _db.Students.FirstOrDefault(s => s.UserID == user.UserID);
        }

        // Dashboard
        public IActionResult Index()
        {
            var student = GetCurrentStudent();
            if (student == null) return Content("Student profile not found.");

            var enrolCount = _db.Enrolments.Count(e => e.StudentID == student.StudentID);
            ViewBag.StudentName = student.FullName;
            ViewBag.EnrolCount = enrolCount;
            return View();
        }

        // 查看可选课程（自己 Programme 下的课）
        public IActionResult Courses()
        {
            var student = GetCurrentStudent();
            if (student == null) return Content("Student profile not found.");

            // 自己专业的所有课
            var available = _db.Courses.Where(c => c.ProgrammeID == student.ProgrammeID).ToList();
            // 自己已经选了的课 ID
            var enrolledIds = _db.Enrolments
                .Where(e => e.StudentID == student.StudentID)
                .Select(e => e.CourseID).ToList();

            ViewBag.EnrolledIds = enrolledIds;
            return View(available);
        }

        // 选课
        public IActionResult Enrol(int id)
        {
            var student = GetCurrentStudent();
            if (student == null) return Content("Student profile not found.");

            // 验证：不能重复选
            bool already = _db.Enrolments.Any(e => e.StudentID == student.StudentID && e.CourseID == id);
            if (!already)
            {
                _db.Enrolments.Add(new Enrolment
                {
                    StudentID = student.StudentID,
                    CourseID = id,
                    EnrolDate = DateTime.Now,
                    Status = "Enrolled"
                });
                _db.SaveChanges();
                TempData["Message"] = "Enrolled successfully!";
            }
            else
            {
                TempData["Message"] = "You are already enrolled in this course.";
            }
            return RedirectToAction("Courses");
        }

        // 退课
        public IActionResult Drop(int id)
        {
            var student = GetCurrentStudent();
            if (student == null) return Content("Student profile not found.");

            var enrolment = _db.Enrolments.FirstOrDefault(e => e.StudentID == student.StudentID && e.CourseID == id);
            if (enrolment != null)
            {
                _db.Enrolments.Remove(enrolment);
                _db.SaveChanges();
                TempData["Message"] = "Dropped course successfully.";
            }
            return RedirectToAction("Courses");
        }

        // 查看考勤
        public IActionResult Attendance()
        {
            var student = GetCurrentStudent();
            if (student == null) return Content("Student profile not found.");

            var records = _db.Attendances.Where(a => a.StudentID == student.StudentID).ToList();
            ViewBag.Courses = _db.Courses.ToList();
            return View(records);
        }

        // 查看成绩（只看已发布的）
        public IActionResult Grades()
        {
            var student = GetCurrentStudent();
            if (student == null) return Content("Student profile not found.");

            var grades = _db.Grades
                .Where(g => g.StudentID == student.StudentID && g.Published == true)
                .ToList();
            ViewBag.Courses = _db.Courses.ToList();
            return View(grades);
        }
    }
}