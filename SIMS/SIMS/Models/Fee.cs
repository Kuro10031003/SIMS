namespace SIMS.Models
{
    public class Fee
    {
        public int FeeID { get; set; }
        public int StudentID { get; set; }
        public decimal Amount { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime? DueDate { get; set; }
        public string Status { get; set; } = "Unpaid";
        public string? Semester { get; set; }
    }
}