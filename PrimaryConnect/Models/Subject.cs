namespace PrimaryConnect.Models
{
    public class Subject
    {
     public int id { get; set; }
     public string Name { get; set; }
        public ICollection<Marks> marks { get; set; }
    }
}
