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

        public int ClassId { get; set; }
        public Class myclass { get; set; }

        public int SchoolId { get; set; }
        public School School { get; set; }
        
        public ICollection<Teacher_Student> Teachers_students { get; set; }


        public ICollection<Courses> Courses { get; set; }
       



        #endregion

    }
}
