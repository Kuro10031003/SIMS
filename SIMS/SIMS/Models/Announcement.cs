namespace SIMS.Models
{
    public class Announcement
    {
        public int AnnouncementID { get; set; }
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public int PostedBy { get; set; }
        public int? CourseID { get; set; }
        public DateTime PostedDate { get; set; }
    }
}