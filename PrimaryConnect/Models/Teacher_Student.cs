using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrimaryConnect.Models
{
    public class Teacher_Student
    {
        [Key]
       public int Id { get; set; }

        [ForeignKey("Teacher")]
        public int Teacher_Id { get; set; }
        public Teacher Teacher { get; set; }
        [ForeignKey("student")]
        public int student_Id { get; set; }
        public Student student { get; set; }

    }
}
