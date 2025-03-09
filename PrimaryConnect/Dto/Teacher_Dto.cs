using PrimaryConnect.Models;

namespace PrimaryConnect.Dto
{
    public class Teacher_Dto:Person
    {
        public string Subject { get; set; }
        public string Class { get; set; }
 public int SchoolId { get; set; }
        public Teacher ToTeacher()
        {
            Teacher teacher = new Teacher();    
            teacher.Subject = Subject;
            teacher.Class = Class;
            teacher.Name = Name;
            teacher.Email = Email;
            teacher.SchoolId = SchoolId;
            teacher.PhoneNumber = PhoneNumber;
            return teacher;
        }

    }
}
