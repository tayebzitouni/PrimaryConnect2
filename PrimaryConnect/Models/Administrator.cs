using System.ComponentModel.DataAnnotations.Schema;

namespace PrimaryConnect.Models
{
    public class Administrator:Person
    {
        
        public int Permitions { get; set; } 


        [ForeignKey("School")]
        public int School_Id { get; set; }
        public School School { get; set; }
    }
}
