using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using PrimaryConnect.Models;


namespace PrimaryConnect.Data
{
    public class AppDbContext: DbContext
    {
       
        public AppDbContext(DbContextOptions<AppDbContext> options)
       : base(options) { 
        
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
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
        public DbSet<ChatMessage> chatMessages { get; set; }
        public DbSet<Teacher_Student> Teacher_Students { get; set; }  
        public DbSet<NotificationRequest> notifications { get; set; }
        public DbSet<Person> person { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Parent>().HasMany(p=>p.students).WithOne(s=>s.parent).HasForeignKey(s=>s.ParentId).OnDelete(DeleteBehavior.Restrict); ;
            modelBuilder.Entity<School>().HasMany<Student>(p => p.students).WithOne(s=>s.school).HasForeignKey(s=>s.SchoolId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<School>().HasMany<Teacher>(p => p.teachers).WithOne(s=>s.School).HasForeignKey(s=>s.SchoolId).OnDelete(DeleteBehavior.Restrict); ; 
            modelBuilder.Entity<School>().HasMany<Administrator>(p => p.administrators).WithOne(s=>s.School).HasForeignKey(s=>s.SchoolId).OnDelete(DeleteBehavior.Restrict); ;
            modelBuilder.Entity<Student>().HasMany<Courses>(p => p.Courses).WithOne(s=>s.student).HasForeignKey(s=>s.StudentId);
            modelBuilder.Entity<Teacher>().HasMany<Teacher_Student>(p => p.Teachers_students).WithOne(s => s.Teacher).HasForeignKey(s => s.TeacherId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Teacher>().HasMany<Courses>(p => p.Courses).WithOne(s => s.Teacher).HasForeignKey(s => s.TeacherId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Student>().HasMany<Teacher_Student>(p => p.Teachers_students).WithOne(s => s.student).HasForeignKey(s => s.StudentId);
            modelBuilder.Entity<Student>().HasMany<Marks>(p => p.marks).WithOne(s => s.student).HasForeignKey(s => s.StudentId);
            modelBuilder.Entity<Student>().HasMany<Absence>(p => p.absences).WithOne(s => s.student).HasForeignKey(s => s.StudentId);
            modelBuilder.Entity<Student>().HasMany<Event_Student>(p => p.envent_student).WithOne(s => s.student).HasForeignKey(s => s.StudentId).OnDelete(DeleteBehavior.Restrict);
         modelBuilder.Entity<Person>().HasMany<ChatMessage>(p => p.chatMessages).WithOne(s => s.person).HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Cascade);
            //modelBuilder.Entity<School>().HasMany<Administrator>(p => p.administrators).WithOne(s => s.School).HasForeignKey(s => s.SchoolId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<School>().HasMany<Student>(p => p.students).WithOne(s => s.school).HasForeignKey(s => s.SchoolId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Event>().HasMany<Event_Student>(p => p.envent_student).WithOne(s => s.Event).HasForeignKey(s => s.EventId);

           

            base.OnModelCreating(modelBuilder);
        }
    }
}
