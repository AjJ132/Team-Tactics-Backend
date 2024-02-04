namespace Team_Tactics_Backend.DTO
{
    public class ReturnCalendarEventDTO
    {
        public string AssigneeName { get; set; }
        public Guid EventId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Color { get; set; }
        public List<string> AssignedUsers { get; set; }
    }
}