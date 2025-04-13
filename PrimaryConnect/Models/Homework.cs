namespace PrimaryConnect.Models
{
    public class Homework
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public string Subject { get; set; }
        public string Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? UserId { get; set; }  // The user to whom the homework is assigned
        public int? ClassId { get; set; }  // The class/group for the homework (optional)
        public bool AssignedToAll { get; set; }  // Flag indicating if assigned to all users of the class
    }
}
