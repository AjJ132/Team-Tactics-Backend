
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamTacticsBackend.Models.CalendarEvents;

public class EventsAssigned
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Index { get; set; }
    public Guid EventId { get; set; }
    public string AssigneeId { get; set; }
    public DateTime DateAssigned { get; set; }

    
    
}


