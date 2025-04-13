namespace PrimaryConnect.Dto
{
    public class ResourceUploadRequest
    {
        public IFormFile File { get; set; }
        public string Subject { get; set; }
        public string TeacherRemark { get; set; }
    }
}
