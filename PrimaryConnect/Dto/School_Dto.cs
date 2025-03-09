using PrimaryConnect.Models;

namespace PrimaryConnect.Dto
{
    public class School_Dto
    {
        public string Name { get; set; }
        public string location { get; set; }
        public  School ToSchool()
        {
School school = new School();
            school.Name = Name;
            school.location = location;
            return school;
        }
    }
}
