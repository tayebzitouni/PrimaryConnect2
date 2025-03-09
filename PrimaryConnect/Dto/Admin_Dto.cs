using Microsoft.AspNetCore.Identity;
using PrimaryConnect.Models;

namespace PrimaryConnect.Dto
{
    public class Admin_Dto:Person
    {
        public int Permitions { get; set; }
        public int School_Id { get; set; }
        public  Administrator ToAdministrator()
        {
            Administrator administrator = new Administrator();
            administrator.Name = Name;
            administrator.Email = Email;
            administrator.Password = Password;
            administrator.SchoolId = School_Id;
            administrator.PhoneNumber = PhoneNumber;
            administrator.Permitions = Permitions;
            



            return administrator;



        }
    }
}
