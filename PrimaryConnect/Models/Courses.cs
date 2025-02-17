using System.ComponentModel.DataAnnotations.Schema;

namespace PrimaryConnect.Models
{
    public class Courses
    {
        #region properties
        public int Id { get; set; }
        public string VideoPath { get; set; }
        public string HomeworkPath { get; set; }
        #endregion
        #region foregnkies
        [ForeignKey("student")]
        public int student_Id { get; set; }
        public Student student { get; set; }
        [ForeignKey("Teacher")]
        public int Teacher_Id { get; set; }
        public Teacher Teacher { get; set; }


        #endregion

    }
}
