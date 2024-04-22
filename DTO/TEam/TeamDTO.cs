namespace TeamTacticsBackend.DTO.Team
{
    public class NewTeamDTO
    {
        public string TeamName { get; set; }
        public string TeamSport { get; set; }
        public string TeamCity { get; set; }
        public string TeamState { get; set; }
    }

    public class ReturnTeamDTO
    {
        public Guid TeamId { get; set; }
        public string? OwnerName { get; set; }
        public string TeamName { get; set; }
        public string TeamSport { get; set; }
        public string TeamCity { get; set; }
        public string TeamState { get; set; }
        public string? TeamJoinCode { get; set; }
        public DateTimeOffset DateCreated { get; set; }

        public List<TeamMemberDTO>? TeamMembers { get; set; }


    }

    public class TeamMemberDTO
    {
        public Guid userId { get; set; }
        public string userName { get; set; }
        public string email { get; set; }

    }
}