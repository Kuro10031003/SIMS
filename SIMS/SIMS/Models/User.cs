namespace SIMS.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";        // "Admin" / "Lecturer" / "Student"
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}