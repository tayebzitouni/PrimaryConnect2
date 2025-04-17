using PrimaryConnect.Models;

namespace PrimaryConnect.Dto
{
    public class Teacher_classDto
    {
        
        


        public int TeacherId { get; set; }
        
        public int [] ClassId { get; set; }

        public Teacher_Class ToSubject()
        {
            Teacher_Class absence = new Teacher_Class();
            absence.TeacherId = TeacherId;
            absence.Assignementdate = DateTime.Now.ToString("yyyy-MM-dd");

            return absence;
        }

    }
}
