namespace TeamTacticsBackend.DTO
{
    public class ReturnCalendarEventDTO
    {
        public string AssigneeName { get; set; }
        public Guid EventId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public string Color { get; set; }
        public Boolean canUpdate { get; set; }
        public string creatorId { get; set; }
        public string creatorName { get; set; }
        public List<string> AssignedUsers { get; set; }
    }
}