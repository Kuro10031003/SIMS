namespace SIMS.Models
{
    public class Course
    {
        public int CourseID { get; set; }
        public string CourseCode { get; set; } = "";
        public string CourseName { get; set; } = "";
        public int Credits { get; set; }
        public int ProgrammeID { get; set; }
        public int? LecturerID { get; set; }
        public string? Semester { get; set; }
    }
}