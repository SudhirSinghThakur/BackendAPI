using BackendAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Data
{
    public class SchoolContext : DbContext
    {
        public SchoolContext(DbContextOptions<SchoolContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        //public DbSet<Exam> Exams { get; set; }
        //public DbSet<TimetableEntry> TimetableEntries { get; set; }
    }
}
