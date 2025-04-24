using PrimaryConnect.Models;

namespace PrimaryConnect.Dto
{
    public class Student_Dto
    {
       
        public string Name { get; set; }
        public int Degree { get; set; }
        public int ParentId { get; set; }
        public int SchoolId { get; set; }
        public int ClassId { get; set; }
        public Student ToStudent()
        {
            Student student = new Student();
           
            student.Name = Name;
            student.Degree = Degree;
            student.ParentId = ParentId;
            student.SchoolId = SchoolId;
            student.ClassId = ClassId;
            return student;
        }
     
    }
}
