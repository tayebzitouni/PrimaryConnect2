using DocumentFormat.OpenXml.Office2013.Excel;

namespace PrimaryConnect.Dto
{
    public class ResourceUploadRequest
    {
        public IFormFile File { get; set; }
        public string Subject { get; set; }
        public string TeacherRemark { get; set; }
        public String date { get; set; }
        public int level { get; set; }
        public bool AssignedToall { get; set; }
    }
}
