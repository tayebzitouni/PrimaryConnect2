using PrimaryConnect.Models;

namespace PrimaryConnect.Dto
{
    public class Parent_Dto:Person
    {
        public override int Id { set; get; }
        public Parent ToParent()
        {
            Parent _parent = new Parent();
           
_parent.Name = this.Name;
_parent.Email = this.Email;
_parent.PhoneNumber = this .PhoneNumber;
_parent.Password = this.Password;

            return _parent;
        } 
    }
}
