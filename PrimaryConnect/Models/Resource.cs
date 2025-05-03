using DocumentFormat.OpenXml.Spreadsheet;

namespace PrimaryConnect.Models
{
    public class Resource
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string FileName { get; set; }
        public string Type { get; set; } // video, document, etc.
        public string TeacherRemark { get; set; }
        public int TeacherId {get;set;}
        public Teacher teacher { get; set; }
        public string date { get; set; }
        public  int level { get; set; }
        public bool AssignedToall { get; set; }
    }
}
