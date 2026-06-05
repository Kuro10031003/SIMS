namespace SIMS.Models
{
    public class Student
    {
        public int StudentID { get; set; }
        public int UserID { get; set; }
        public int ProgrammeID { get; set; }
        public string FullName { get; set; } = "";
        public DateTime? DateOfBirth { get; set; }
        public string? Phone { get; set; }
        public DateTime EnrollDate { get; set; }
        public string Status { get; set; } = "Active";
    }
}