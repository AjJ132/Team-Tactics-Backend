

namespace TeamTacticsBackend.DTO;

public class NewCalendarEventDTO
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public string Color { get; set; }
    public bool assignMe { get; set; }
    public List<string> UserIds { get; set; }
}