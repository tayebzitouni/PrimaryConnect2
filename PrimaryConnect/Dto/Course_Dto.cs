using PrimaryConnect.Models;

namespace PrimaryConnect.Dto
{
    public class Course_Dto
    {
        public int Id { get; set; }
        public string VideoPath { get; set; }
        public string HomeworkPath { get; set; }

     
        public int StudentId { get; set; }

        public int TeacherId { get; set; }
        public Courses ToCourse()
        {
            Courses courses = new Courses();
            courses.Id = Id;
                courses.VideoPath = VideoPath;
            courses.HomeworkPath = HomeworkPath;
            courses.StudentId = StudentId;
            courses.TeacherId = TeacherId;
            return courses;
        }
    }
}
