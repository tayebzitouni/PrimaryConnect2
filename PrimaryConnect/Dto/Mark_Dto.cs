using PrimaryConnect.Data;
using PrimaryConnect.Models;
using System.ComponentModel.DataAnnotations;

namespace PrimaryConnect.Dto
{
    



    public class Mark_Dto
    {
        public int Id { get; set; }
        [Required]
        public UsefulFunctions.Subject subject { get; set; }
        public int Mark{ get; set; }
       
       
        public String Remarque { get; set; }
        public int Semestre { get; set; }
        public int Year { get; set; }


        public int StudentId { get; set; }
        public Marks ToMark()
        {
            Marks marks = new Marks();  
            marks.Id = Id;  
            marks.Subject = subject;
            marks.Mark = Mark;
            marks.StudentId = StudentId;
            marks.Remarque = Remarque;
            marks.Semestre = Semestre;
            marks.Year = Year;
            return marks;
        }

    }
}
