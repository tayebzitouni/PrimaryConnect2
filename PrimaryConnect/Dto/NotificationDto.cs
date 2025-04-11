namespace PrimaryConnect.Dto
{
    public class NotificationDto
    {
            public int SenderId { get; set; }
            public string Title { get; set; }
            public string Body { get; set; }

            public bool SendToAll { get; set; }

            public List<string>? Roles { get; set; }       // e.g., ["Teacher"]
            public List<string>? Classes { get; set; }     // e.g., ["Class A"]
            public List<int>? UserIds { get; set; }       // e.g., specific user IDs
    }
}
