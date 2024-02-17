

namespace TeamTacticsBackend.DTO;

public class NewCalendarEventDTO
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Color { get; set; }
    public List<string> UserIds { get; set; }
}