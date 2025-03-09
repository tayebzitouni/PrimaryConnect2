using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrimaryConnect.Models
{
    public class Teacher_Student
    {
        [Key]
       public int Id { get; set; }

       
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }
       
        public int StudentId { get; set; }
        public Student student { get; set; }

    }
}
