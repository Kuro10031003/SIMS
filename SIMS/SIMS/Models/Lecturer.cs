namespace SIMS.Models
{
    public class Lecturer
    {
        public int LecturerID { get; set; }
        public int UserID { get; set; }
        public string FullName { get; set; } = "";
        public string? Phone { get; set; }
        public string? Department { get; set; }
    }
}