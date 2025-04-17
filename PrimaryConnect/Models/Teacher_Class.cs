using System.ComponentModel.DataAnnotations;

namespace PrimaryConnect.Models
{
    public class Teacher_Class
    {
        [Key]
        public int Id { get; set; }

        public string Assignementdate { get; set; }

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }

        public int ClassId { get; set; }
        public Class Class { get; set; }

    }
}
