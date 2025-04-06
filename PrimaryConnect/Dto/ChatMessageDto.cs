using PrimaryConnect.Models;
using System.ComponentModel.DataAnnotations;

namespace PrimaryConnect.Dto
{
    public class ChatMessageDto
    {
            public string Text { get; set; }
            public int Id { get; set; }

        public int UserId { get; set; }
        public ChatMessage TochatMessage()
        {
            ChatMessage marks = new ChatMessage();
            marks.Id = Id;
            marks.Text = Text;
            marks.UserId = UserId;
            return marks;
        }
    }
}