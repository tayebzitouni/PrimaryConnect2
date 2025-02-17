using System.ComponentModel.DataAnnotations.Schema;

namespace PrimaryConnect.Models
{
    public class Event
    {
        #region properties 
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime Date { get; set; }
        #endregion
        #region foreingkeys

        public ICollection<Event_Student> envent_student { get; set; }
        #endregion

    }
}
