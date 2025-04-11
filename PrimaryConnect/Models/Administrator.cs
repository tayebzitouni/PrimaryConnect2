using System.ComponentModel.DataAnnotations.Schema;

namespace PrimaryConnect.Models
{
    public class Administrator
    {
        
        public int id { get; set; }
        public int Permitions { get; set; } 


        public int SchoolId { get; set; }
        public School School { get; set; }

        public int personId { get; set; }
        public Person person { get; set; }
        
    }
}
