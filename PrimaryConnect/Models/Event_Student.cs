using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrimaryConnect.Models
{
    public class Event_Student
    {
        [Key]
        public int Id { get; set; }
        #region Foregnkies 

        [ForeignKey("student")]
        public int student_Id { get; set; }
        public Student student { get; set; }

        [ForeignKey("Event_P")]
        public int Event_Id { get; set; }
        public Event Event_P { get; set; }
        #endregion
    }
}
