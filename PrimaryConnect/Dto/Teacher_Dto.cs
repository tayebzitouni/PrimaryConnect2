using PrimaryConnect.Models;

namespace PrimaryConnect.Dto
{
    public class Teacher_Dto:PersonDto
    {
        public string Subject { get; set; }
        public int Classid { get; set; }
 public int SchoolId { get; set; }

        public Teacher ToTeacher()
        {
            Teacher teacher = new Teacher();    
            teacher.Subject = Subject;
            teacher.ClassId = Classid;
            teacher.Password = Password;
            teacher.Name = Name;
            teacher.Email = Email;
            teacher.SchoolId = SchoolId;
            teacher.PhoneNumber = PhoneNumber;
            return teacher;
        }

    }
}
