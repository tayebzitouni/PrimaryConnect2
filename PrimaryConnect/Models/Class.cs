namespace PrimaryConnect.Models
{
    public class Class
    {
    public int id { get; set; }
    public string name { get; set; }
    public int SchoolId { get; set; }
    public School school { get; set; }
    public int level{ get; set; }
   
    public ICollection<Student>students { get; set; }
    public ICollection<Teacher> teachers { get; set; }
     public ICollection<Teacher_Class> teacher_Classes { get; set; }
    }
}
