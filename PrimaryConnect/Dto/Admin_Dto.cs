using Microsoft.AspNetCore.Identity;
using PrimaryConnect.Models;

namespace PrimaryConnect.Dto
{
    public class Admin_Dto
    {
              public int id { get; set; }
        public int Permitions { get; set; }
        public int School_Id { get; set; }
       public int personId { get; set; }
       }
}
