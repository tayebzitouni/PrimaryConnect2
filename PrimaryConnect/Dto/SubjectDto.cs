using PrimaryConnect.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PrimaryConnect.Dto
{
    public class SubjectDto
    {
        public int id { get; set; }
        public string Name { get; set; }
        public Subject ToSubject()
        {
            Subject absence = new Subject();

            absence.id = id;
            absence.Name = Name;
            return absence;
        }
    }
}
