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
 
        //
        //
     public int StudentId { get; set; }
        public Student student { get; set; }
       
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }


        #endregion

    }
}
