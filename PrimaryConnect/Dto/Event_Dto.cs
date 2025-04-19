
using PrimaryConnect.Models;

namespace PrimaryConnect.Dto
{
    public class Event_Dto
    {
        #region properties 
       
        public string title { get; set; }
        public string Description { get; set; }
        public DateOnly date { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public bool All { get; set; }
        public int level { get; set; }
        public string Color { get; set; }
        #endregion
        public Event ToEvent()
        {
              Event _event = new Event();
            
            _event.title = title;
            _event.Description = Description;
            _event.date = date;
            _event.StartTime = StartTime;
            _event.EndTime = EndTime;
            _event.All = All;
            _event.level = level;
            _event.Color = Color;
            return _event;
        }
    }
}
