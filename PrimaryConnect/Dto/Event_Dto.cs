
using PrimaryConnect.Models;

namespace PrimaryConnect.Dto
{
    public class Event_Dto
    {
        #region properties 
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime Date { get; set; }
        #endregion
        public Event ToEvent()
        {
              Event _event = new Event();
            _event.Id = Id; 
            _event.Name = Name;
            _event.Location = Location;
                _event.Date = Date;
            return _event;
        }
    }
}
