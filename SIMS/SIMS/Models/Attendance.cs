namespace SIMS.Models
{
    public class Attendance
    {
        public int AttendanceID { get; set; }
        public int StudentID { get; set; }
        public int CourseID { get; set; }
        public DateTime AttDate { get; set; }
        public string Status { get; set; } = "";
        public int? RecordedBy { get; set; }
    }
}