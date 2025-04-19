using System.ComponentModel.DataAnnotations.Schema;

namespace PrimaryConnect.Models
{
    public class Event
    {
        #region properties 
        public int Id { get; set; }
        public string title { get; set; }
        public string Description { get; set; }
        public DateOnly date { get; set; }
        public string  StartTime { get; set; }
        public string EndTime { get; set; }
        public bool All { get; set; }
        public int level { get; set; }
        public String Color { get; set; }
        #endregion
        #region foreingkeys

        public ICollection<Event_Student> envent_student { get; set; }
        #endregion

    }
}
