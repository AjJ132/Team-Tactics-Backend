using System;
using System.ComponentModel.DataAnnotations;

namespace TeamTacticsBackend.Models.CalendarEvents
{
    public class CalendarEvent
    {
        [Key]
        public Guid EventId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate {get; set; }
        public DateTime EndDate { get; set; }
        public string Color { get; set; }
        public string CreatorId { get; set; }
        public DateTime DateCreated { get; set; }

        public CalendarEvent()
        {
            EventId = Guid.NewGuid();
            DateCreated = DateTime.Now;
        }

    }
}
