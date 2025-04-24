using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrimaryConnect.Models
{
    // In Models/ChatMessage.cs
    public class ChatMessage
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string UserName { get; set; }  // Make sure this exists
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // If you're using UserId (from authentication)
        public int? UserId { get; set; }  // Make nullable if not always available
    }
}