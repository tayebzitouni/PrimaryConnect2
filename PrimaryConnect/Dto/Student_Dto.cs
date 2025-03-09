using PrimaryConnect.Models;

namespace PrimaryConnect.Dto
{
    public class Student_Dto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Degree { get; set; }
        public int ParentId { get; set; }
        public int SchoolId { get; set; }
        public Student ToStudent()
        {
            Student student = new Student();
            student.Id = Id;
            student.Name = Name;
            student.Degree = Degree;
            student.ParentId = ParentId;
            student.SchoolId = SchoolId;
            return student;
        }
     
    }
}
