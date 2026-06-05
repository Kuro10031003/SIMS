namespace SIMS.Models
{
    public class Grade
    {
        public int GradeID { get; set; }
        public int StudentID { get; set; }
        public int CourseID { get; set; }
        public string AssessType { get; set; } = "";
        public decimal Marks { get; set; }
        public decimal MaxMarks { get; set; }
        public string? GradeLetter { get; set; }
        public decimal? GPA { get; set; }
        public bool Published { get; set; }
    }
}