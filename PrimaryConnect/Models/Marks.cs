using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrimaryConnect.Models
{
    public class Marks
    {
        
        #region Properties 
        [Key]
        public int Id { get; set; } 
    public string Subject {  get; set; }    
        public int Mark
        { get; set; }
        #endregion
        #region ForeignKeies

            [ForeignKey("student")]
            public int student_Id { get; set; }
            public Student student { get; set; }

        #endregion



    }
}
