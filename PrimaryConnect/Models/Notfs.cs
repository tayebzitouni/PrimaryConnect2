namespace PrimaryConnect.Models
{
      public class Notification
        {
            public int Id { get; set; }  // Unique ID for the notification
            public string SenderId { get; set; }  // ID of the sender (who is sending the notification)
            public string RecipientId { get; set; }  // ID of the recipient (who will receive the notification)
            public string RecipientType { get; set; }  // Role of the recipient (e.g., admin, teacher, parent)
            public string Title { get; set; }  // Title of the notification
            public string Message { get; set; }  // Body/content of the notification
            public bool IsRead { get; set; }  // Whether the notification has been read
            public DateTime SentAt { get; set; } = DateTime.UtcNow;  // Timestamp when the notification was sent
        }
}
