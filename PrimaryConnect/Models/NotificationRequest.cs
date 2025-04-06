namespace PrimaryConnect.Models
{
    public class NotificationRequest
    {
        public string Title { get; set; }
        public string Message { get; set; }

        public bool SendToAll { get; set; } = false;
        public List<string>? TargetRoles { get; set; } // Example: ["teacher", "parent"]
        public List<string>? TargetClasses { get; set; } // Example: ["Class A", "Class B"]
        public List<string>? UserIds { get; set; } // Example: ["teacher1-id", "parent3-id"]
    }

}
