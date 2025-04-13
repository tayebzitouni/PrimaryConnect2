using Microsoft.Identity.Client;
using PrimaryConnect.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrimaryConnect.Models
{
    public class Marks
    {
        
        #region Properties 
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public UsefulFunctions.Subject Subject {  get; set; }    
        public int Mark
        { get; set; }
        public String Remarque { get; set; }
        public int Semestre { get; set; }
        public int Year { get; set; } 
         
        #endregion
        #region ForeignKeies

         
          public int StudentId { get; set; }
            public Student student { get; set; }

        #endregion



    }
}
