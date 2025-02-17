using Microsoft.EntityFrameworkCore;

namespace PrimaryConnect.Models
{
    public class Parent:Person
    {
        #region Properties 
        #endregion 
       


        
        #region ForeignKeies
        public ICollection<Student> students { get; set; }
      
        
        #endregion
    }
}
