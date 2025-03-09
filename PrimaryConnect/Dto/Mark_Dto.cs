using PrimaryConnect.Models;

namespace PrimaryConnect.Dto
{
    public class Mark_Dto
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public int Mark{ get; set; }

        public int StudentId { get; set; }
        public Marks ToMark()
        {
            Marks marks = new Marks();  
            marks.Id = Id;  
            marks.Subject = Subject;
            marks.Mark = Mark;
            marks.StudentId = StudentId;
            return marks;
        }

    }
}
