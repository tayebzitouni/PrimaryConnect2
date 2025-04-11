using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrimaryConnect.Models
{
    public class ChatMessage
    {
        [Key]
        public int  Id { get; set; }
        public string Text { get; set; }
        public DateTime datetime { get; set; }
        #region foregnkeys
        public int UserId { get; set; }
        public Person person { get; set; }
        #endregion
    }
}
