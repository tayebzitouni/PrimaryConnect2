using PrimaryConnect.Models;

namespace PrimaryConnect.Dto
{
    public class ClassDto
    {
        public int id { get; set; }
        public string Name { get; set; }
        public int schoolid { get; set; }
        public int level { get; set; }
        public Class ClassDtoToClass()
        {
            Class school = new Class();
            school.name = Name;
            school.level = level;
            school.SchoolId = schoolid;
            return school;
        }
    }
}
