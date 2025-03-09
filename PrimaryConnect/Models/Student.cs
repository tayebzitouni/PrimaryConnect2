using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PrimaryConnect.Models
{
    public class Student
    {
        #region properties
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public int Degree {  get; set; }

        #endregion
        #region Foregnkeys 

        public int ParentId { get; set; }
        public Parent parent { get; set; }

        public int SchoolId { get; set; }
        public School school { get; set; }

        public ICollection<Marks> marks { get; set; }
        public ICollection<Absence> absences { get; set; }
        public ICollection< Courses> Courses { get; set; }
        public ICollection<Teacher_Student> Teachers_students { get; set; }
        public ICollection  <Event_Student> envent_student { get; set; }
        #endregion
    }
}
