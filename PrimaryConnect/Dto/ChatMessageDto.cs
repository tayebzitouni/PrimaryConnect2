using PrimaryConnect.Models;
using System.ComponentModel.DataAnnotations;

namespace PrimaryConnect.Dto
{
    public class ChatMessageDto
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public string UserName { get; set; }
    }
}

