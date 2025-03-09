using PrimaryConnect.Models;

namespace PrimaryConnect.Dto
{
    public class Absence_Dto
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public DateTime Date { get; set; }
        public bool IsJustified { get; set; }

        public int StudentId { get; set; }

        public Absence ToAbsence()
        { 
            Absence absence =new Absence();
       
            absence.Id = Id;
            absence.Subject = Subject;
            absence.Date = Date;
             absence.StudentId = StudentId;
            absence.IsJustified = IsJustified;
            return absence;
        }
        
    }
}
