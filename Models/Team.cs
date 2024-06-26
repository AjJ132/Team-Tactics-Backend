using System.ComponentModel.DataAnnotations;

namespace TeamTacticsBackend.Models.Teams;

public class Team
{
    [Key]
    public Guid TeamId { get; set; }
    public string OwnerId { get; set; }
    public string TeamName { get; set; }
    public string TeamSport { get; set; }
    public string TeamCity { get; set; }
    public string TeamState { get; set; }
    public string? TeamJoinCode { get; set; }
    public DateTimeOffset DateCreated { get; set; }

    public Team()
    {
        TeamId = Guid.NewGuid();
        DateCreated = DateTime.Now;
    }    
}
