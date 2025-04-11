using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;


namespace PrimaryConnect.Models
{
    public class Person
    {
        [Key]
        virtual public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }  
        public string PhoneNumber { get; set; }
        public string FcmToken { get; set; }

        #region foregnkeys
        public ICollection<ChatMessage> ?chatMessages { get; set; }
        public Administrator admin { get; set; }
        #endregion
    }
}
