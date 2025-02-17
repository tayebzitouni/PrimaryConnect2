using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using PrimaryConnect.Models;


namespace PrimaryConnect.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<Marks> marks { get; set; } 
        public DbSet<Event> events { get; set; }
        public DbSet<Courses> courses { get; set; }
        public DbSet<School> schools { get; set; }  
        public  DbSet<Event_Student> events_Students { get; set; }
        public DbSet<Absence>   absences { get; set; }
        public DbSet<Teacher_Student> Teacher_Students { get; set; }    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Parent>().HasMany<Student>().WithOne(s=>s.parent).HasForeignKey(s=>s.Parent_Id);
       modelBuilder.Entity<School>().HasMany<Student>().WithOne(s=>s.school).HasForeignKey(s=>s.School_Id);
              modelBuilder.Entity<School>().HasMany<Teacher>().WithOne(s=>s.School).HasForeignKey(s=>s.School_Id); 
            modelBuilder.Entity<School>().HasMany<Administrator>().WithOne(s=>s.School).HasForeignKey(s=>s.School_Id);
            modelBuilder.Entity<Student>().HasMany<Courses>().WithOne(s=>s.student).HasForeignKey(s=>s.student_Id).OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
