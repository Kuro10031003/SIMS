using Microsoft.EntityFrameworkCore;
using SIMS.Models;

namespace SIMS.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // All tables in the SIMS database
        public DbSet<User> Users { get; set; }
        public DbSet<Programme> Programmes { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrolment> Enrolments { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Fee> Fees { get; set; }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            // Map each model to its SQL table name
            mb.Entity<User>().ToTable("Users");
            mb.Entity<Programme>().ToTable("Programmes");
            mb.Entity<Student>().ToTable("Students");
            mb.Entity<Lecturer>().ToTable("Lecturers");
            mb.Entity<Course>().ToTable("Courses");
            mb.Entity<Enrolment>().ToTable("Enrolments");
            mb.Entity<Attendance>().ToTable("Attendance");
            mb.Entity<Grade>().ToTable("Grades");
            mb.Entity<Announcement>().ToTable("Announcements");
            mb.Entity<Fee>().ToTable("Fees");
        }
    }
}