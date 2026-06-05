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

        // 考勤 - 显示某门课的学生，让讲师标记（GET）
        public IActionResult TakeAttendance(int id)
        {
            var lecturer = GetCurrentLecturer();
            if (lecturer == null) return Content("Lecturer profile not found.");

            var course = _db.Courses.FirstOrDefault(c => c.CourseID == id && c.LecturerID == lecturer.LecturerID);
            if (course == null) return Content("Course not found or not yours.");

            var enrolments = _db.Enrolments.Where(e => e.CourseID == id).ToList();
            var studentIds = enrolments.Select(e => e.StudentID).ToList();
            var students = _db.Students.Where(s => studentIds.Contains(s.StudentID)).ToList();

            ViewBag.CourseID = course.CourseID;
            ViewBag.CourseName = course.CourseName;
            ViewBag.CourseCode = course.CourseCode;
            return View(students);
        }

        // 考勤 - 保存标记（POST）
        [HttpPost]
        public IActionResult TakeAttendance(int courseId, DateTime attDate, int[] studentId, string[] status)
        {
            var lecturer = GetCurrentLecturer();
            if (lecturer == null) return Content("Lecturer profile not found.");

            // 逐个学生保存考勤记录
            for (int i = 0; i < studentId.Length; i++)
            {
                var attendance = new Attendance
                {
                    StudentID = studentId[i],
                    CourseID = courseId,
                    AttDate = attDate,
                    Status = status[i],
                    RecordedBy = lecturer.LecturerID
                };
                _db.Attendances.Add(attendance);
            }
            _db.SaveChanges();

            TempData["Message"] = "Attendance recorded successfully!";
            return RedirectToAction("Courses");
        }

        // 录入成绩 - 显示学生（GET）
        public IActionResult EnterGrades(int id)
        {
            var lecturer = GetCurrentLecturer();
            if (lecturer == null) return Content("Lecturer profile not found.");

            var course = _db.Courses.FirstOrDefault(c => c.CourseID == id && c.LecturerID == lecturer.LecturerID);
            if (course == null) return Content("Course not found or not yours.");

            var enrolments = _db.Enrolments.Where(e => e.CourseID == id).ToList();
            var studentIds = enrolments.Select(e => e.StudentID).ToList();
            var students = _db.Students.Where(s => studentIds.Contains(s.StudentID)).ToList();

            ViewBag.CourseID = course.CourseID;
            ViewBag.CourseName = course.CourseName;
            ViewBag.CourseCode = course.CourseCode;
            return View(students);
        }

        // 录入成绩 - 保存（POST）
        [HttpPost]
        public IActionResult EnterGrades(int courseId, string assessType, int[] studentId, decimal[] marks, decimal maxMarks)
        {
            for (int i = 0; i < studentId.Length; i++)
            {
                var grade = new Grade
                {
                    StudentID = studentId[i],
                    CourseID = courseId,
                    AssessType = assessType,
                    Marks = marks[i],
                    MaxMarks = maxMarks,
                    GradeLetter = CalculateGrade(marks[i], maxMarks),
                    Published = false   // 默认不发布，要另外点发布
                };
                _db.Grades.Add(grade);
            }
            _db.SaveChanges();

            TempData["Message"] = "Grades saved! Remember to publish them so students can see.";
            return RedirectToAction("Courses");
        }

        // 辅助方法：根据分数算等级
        private string CalculateGrade(decimal marks, decimal max)
        {
            decimal pct = (marks / max) * 100;
            if (pct >= 80) return "A";
            if (pct >= 70) return "B";
            if (pct >= 60) return "C";
            if (pct >= 50) return "D";
            return "F";
        }

        // 查看 + 发布某门课的成绩
        public IActionResult Grades(int id)
        {
            var lecturer = GetCurrentLecturer();
            if (lecturer == null) return Content("Lecturer profile not found.");

            var course = _db.Courses.FirstOrDefault(c => c.CourseID == id && c.LecturerID == lecturer.LecturerID);
            if (course == null) return Content("Course not found or not yours.");

            var grades = _db.Grades.Where(g => g.CourseID == id).ToList();
            ViewBag.CourseID = course.CourseID;
            ViewBag.CourseCode = course.CourseCode;
            ViewBag.Students = _db.Students.ToList();   // 用来显示学生名字
            return View(grades);
        }

        // 发布某门课的所有成绩
        public IActionResult PublishGrades(int id)
        {
            var grades = _db.Grades.Where(g => g.CourseID == id).ToList();
            foreach (var g in grades)
                g.Published = true;
            _db.SaveChanges();

            TempData["Message"] = "Grades published! Students can now see them.";
            return RedirectToAction("Grades", new { id = id });
        }
    }
}