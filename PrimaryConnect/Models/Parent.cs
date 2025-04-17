using Microsoft.EntityFrameworkCore;

namespace PrimaryConnect.Models
{
    public class Parent:Person
    {
        #region Properties 
        
        #endregion 
       


        
        #region ForeignKeies
        public ICollection<Student> students { get; set; }
        public ICollection<Parent> parents { get; set; }
        public ICollection<RequestDocument> requests { get; set; }
        #endregion
    }
}
