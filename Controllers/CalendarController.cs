
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Team_Tactics_Backend.Models.Users;
using Team_Tactics_Backend.Database;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using Team_Tactics_Backend.DTO;
using TeamTacticsBackend.Models.CalendarEvents;
using System.Diagnostics;


namespace Team_Tactics_Backend.CalendarControllers
{

    [ApiController]
    [Route("[controller]")]
    public class CalendarController : ControllerBase
    {
        private readonly UserManager<User> usermanager;
        private readonly IDbContextFactory<TeamTacticsDBContext> contextFactory;

        public CalendarController(UserManager<User> UserManager, IDbContextFactory<TeamTacticsDBContext> contextFactory)
        {
            this.usermanager = UserManager;
            this.contextFactory = contextFactory;
        }

        //Create calendar event
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateCalendarEvent([FromBody] NewCalendarEventDTO model)
        {
            try
            {
                //Get the current user
                var identUser = await usermanager.GetUserAsync(User);

                if (identUser == null)
                {
                    return BadRequest("User not found");
                }

                using (var db = contextFactory.CreateDbContext())
                {
                    //Get the assignee user (this)
                    var user = await db.Users.FirstOrDefaultAsync(u => u.Id == identUser.Id);

                    if (user == null)
                    {
                        return BadRequest("User not found");
                    }

                    //Create the calendar event
                    var calendarEvent = new CalendarEvent
                    {
                        Title = model.Title,
                        Description = model.Description,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate,
                        Color = model.Color,
                        CreatorId = user.Id
                    };

                    //create new GUID
                    calendarEvent.EventId = Guid.NewGuid();

                    List<EventsAssigned> assignees = new List<EventsAssigned>();

                    //Assign users
                    foreach (var userId in model.UserIds)
                    {
                        var assignedUser = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);

                        if (assignedUser == null)
                        {
                            Debug.WriteLine("User not found when adding calendar event");
                            continue;
                        }

                        EventsAssigned newAssign = new EventsAssigned
                        {
                            EventId = calendarEvent.EventId,
                            AssigneeId = assignedUser.Id,
                            DateAssigned = DateTime.Now
                        };

                        assignees.Add(newAssign);
                    }

                    //Add the event to the database
                    db.CalendarEvents.Add(calendarEvent);
                    db.EventsAssigneds.AddRange(assignees);

                    //Save the changes
                    await db.SaveChangesAsync();

                    //create new DTO
                    var calendarEventDTO = new ReturnCalendarEventDTO
                    {
                        EventId = calendarEvent.EventId,
                        Title = calendarEvent.Title,
                        Description = calendarEvent.Description,
                        StartDate = calendarEvent.StartDate,
                        EndDate = calendarEvent.EndDate,
                        Color = calendarEvent.Color,
                        AssigneeName = user.FirstName + " " + user.LastName,
                        AssignedUsers = new List<string>()
                    };

                    //Add the assigned users to the DTO
                    foreach (var assignee in assignees)
                    {
                        var assignedUser = await db.Users.FirstOrDefaultAsync(u => u.Id == assignee.AssigneeId);

                        if (assignedUser == null)
                        {
                            Debug.WriteLine("User not found when adding calendar event");
                            continue;
                        }

                        calendarEventDTO.AssignedUsers.Add(assignedUser.FirstName + " " + assignedUser.LastName);

                    }

                    //Return the event
                    return Ok(calendarEventDTO);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        //Update calendar event

    }

}
