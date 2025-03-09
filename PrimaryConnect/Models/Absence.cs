using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrimaryConnect.Models
{
    public class Absence
    {
        #region Properties 
        [Key]
        public int Id { get; set; }
        public string Subject { get; set; }
        public DateTime Date { get; set; }
        public bool IsJustified { get; set; }
        #endregion
        #region ForeignKeies
      
        public int StudentId { get; set; }
        public Student student { get; set; }

        #endregion


    }
}
