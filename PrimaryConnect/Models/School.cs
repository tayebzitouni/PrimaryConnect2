using Microsoft.EntityFrameworkCore;    
namespace PrimaryConnect.Models
{
    public class School
    {
        #region properties
        public int Id { get; set; }
        public string Name { get; set; }
        public string location { get; set; }
        #endregion
        #region foreignKeys
        public ICollection<Administrator>   administrators { get; set; }
        public ICollection <Teacher> teachers { get; set; } 
        public  ICollection< Student> students { get; set; }
        public ICollection<Class> classs { get; set; }


        #endregion
    }
}
