

using System.ComponentModel.DataAnnotations;

namespace Team_Tactics_Backend.Models.Users;

public class User
{
    [Key]
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }    
    public int UserType { get; set; }
    public Guid TeamId { get; set; }
    public DateTime DateJoined { get; set; }
}

