

using System.ComponentModel.DataAnnotations;
using TeamTacticsBackend.Models.CalendarEvents;

namespace TeamTacticsBackend.Models.Users
{

public class User
{
    [Key]
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }    
    public int UserType { get; set; }
    public Guid TeamId { get; set; }
    public DateTime DateJoined { get; set; }

    public virtual ICollection <CalendarEvent> Events { get; set; }
}

 
}
