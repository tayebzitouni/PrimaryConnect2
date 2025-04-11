namespace PrimaryConnect.Models
{
    public class NotificationRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public int SenderId { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool SendToAll { get; set; } = false;
        public List<string>? TargetRoles { get; set; } // Example: ["teacher", "parent"]
        public List<string>? TargetClasses { get; set; } // Example: ["Class A", "Class B"]
        public List<string>? UserIds { get; set; } // Example: ["teacher1-id", "parent3-id"]
        
    }

}
