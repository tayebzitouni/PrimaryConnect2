using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrimaryConnect.Models
{
    public class Event_Student
    {
        [Key]
        public int Id { get; set; }
        #region Foregnkies 

 
        public int StudentId { get; set; }
        public Student student { get; set; }

       public int EventId { get; set; }
        public Event Event { get; set; }
        #endregion
    }
}
