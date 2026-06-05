namespace SIMS.Models
{
    public class Enrolment
    {
        public int EnrolmentID { get; set; }
        public int StudentID { get; set; }
        public int CourseID { get; set; }
        public DateTime EnrolDate { get; set; }
        public string Status { get; set; } = "Enrolled";
    }
}