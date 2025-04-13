namespace PrimaryConnect.Dto
{
    public class HomeworkUploadRequest
    {
      
            public IFormFile File { get; set; }
            public string Subject { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public int? UserId { get; set; }
            public int? ClassId { get; set; }
            public bool AssignedToAll { get; set; }
        
        // True if it's for the whole class
    }
}
