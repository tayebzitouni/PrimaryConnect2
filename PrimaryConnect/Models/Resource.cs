namespace PrimaryConnect.Models
{
    public class Resource
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string FileName { get; set; }
        public string Type { get; set; } // video, document, etc.
        public string TeacherRemark { get; set; }
    }
}
