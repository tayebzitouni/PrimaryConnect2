using System.ComponentModel.DataAnnotations.Schema;

namespace PrimaryConnect.Models
{
    public class Administrator:Person
    {
        
        public int Permitions { get; set; } 


        public int SchoolId { get; set; }
        public School School { get; set; }
        
    }
}
