using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
namespace PrimaryConnect.Models
{
    public class Teacher:Person
    {
        #region Properties 
        public string Subject { get; set; }
        #endregion
        #region ForeignKeies

        
        public int SchoolId { get; set; }
        public School School { get; set; }
        
        public ICollection<Teacher_Student> Teachers_students { get; set; }

        public ICollection<Teacher_Class> Teacher_class { get; set; }

        public ICollection<Courses> Courses { get; set; }

        public ICollection<RequestDocument> requests { get; set; }



        #endregion

    }
}
