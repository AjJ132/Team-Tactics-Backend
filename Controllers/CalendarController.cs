
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamTacticsBackend.Models.Users;
using TeamTacticsBackend.Database;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using TeamTacticsBackend.DTO;
using TeamTacticsBackend.Models.CalendarEvents;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;


namespace TeamTacticsBackend.CalendarControllers
{

    [ApiController]
    [Route("[controller]")]
    public class CalendarController : ControllerBase
    {
        private readonly UserManager<IdentityUser> usermanager;
        private readonly IDbContextFactory<TeamTacticsDBContext> contextFactory;

        public CalendarController(UserManager<IdentityUser> UserManager, IDbContextFactory<TeamTacticsDBContext> contextFactory)
        {
            this.usermanager = UserManager;
            this.contextFactory = contextFactory;
        }

        //Get my calendar events
        [HttpGet("myevents")]
        [Authorize]
        public async Task<IActionResult> GetMyCalendarEvents()
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
                    //Get the user
                    var user = await db.Users.FirstOrDefaultAsync(u => u.Id == identUser.Id);

                    if (user == null)
                    {
                        return BadRequest("User not found");
                    }

                    //Get the events assigned to the user
                    var eventsAssigned = await db.EventsAssigneds.Where(e => e.AssigneeId == user.Id).ToListAsync();

                    List<ReturnCalendarEventDTO> calendarEvents = new List<ReturnCalendarEventDTO>();

                    //Get the events
                    foreach (var eventAssigned in eventsAssigned)
                    {
                        var calendarEvent = await db.CalendarEvents.FirstOrDefaultAsync(e => e.EventId == eventAssigned.EventId);

                        if (calendarEvent == null)
                        {
                            Debug.WriteLine("Event not found when getting my events");
                            continue;
                        }

                        var creator = await db.Users.FirstOrDefaultAsync(u => u.Id == calendarEvent.CreatorId);

                        //Create the DTO
                        var calendarEventDTO = new ReturnCalendarEventDTO
                        {
                            EventId = calendarEvent.EventId,
                            Title = calendarEvent.Title,
                            Description = calendarEvent.Description,
                            StartDate = calendarEvent.StartDate.DateTime,
                            EndDate = calendarEvent.EndDate.DateTime,
                            Color = calendarEvent.Color,
                            AssigneeName = creator.FirstName + " " + creator.LastName,
                            AssignedUsers = new List<string>(),
                            creatorName = creator.FirstName + " " + creator.LastName,
                            creatorId = user.Id
                        };

                        //Get the assigned users
                        var assignedUsers = await db.EventsAssigneds.Where(e => e.EventId == calendarEvent.EventId).ToListAsync();

                        foreach (var assignedUser in assignedUsers)
                        {
                            var userAssigned = await db.Users.FirstOrDefaultAsync(u => u.Id == assignedUser.AssigneeId);

                            if (userAssigned == null)
                            {
                                Debug.WriteLine("User not found when getting my events");
                                continue;
                            }
                        }

                       //add calendar event to the dto list
                       calendarEvents.Add(calendarEventDTO);
                    }

                    return Ok(calendarEvents);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return StatusCode(500, ex.Message);
            }
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

                    DateTimeOffset utcNow = DateTimeOffset.UtcNow;

                    var startDate = DateTime.Parse(model.StartDate);
                    var endDate = DateTime.Parse(model.EndDate);

                    var startDateUtc = startDate.ToUniversalTime();
                    var endDateUtc = endDate.ToUniversalTime();

                    //Create the calendar event
                    var calendarEvent = new CalendarEvent
                    {
                        Title = model.Title,
                        Description = model.Description,
                        StartDate = startDateUtc,
                        EndDate = endDateUtc,
                        Color = model.Color,
                        DateCreated = DateTime.UtcNow,
                        CreatorId = user.Id
                    };

                    //create new GUID
                    calendarEvent.EventId = Guid.NewGuid();
                    db.CalendarEvents.Add(calendarEvent);
                    await db.SaveChangesAsync();

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
                            DateAssigned = DateTime.UtcNow
                        };

                        assignees.Add(newAssign);
                    }

                    //if add me then add the current user
                    if (model.assignMe == true)
                    {
                        EventsAssigned newAssign = new EventsAssigned
                        {
                            EventId = calendarEvent.EventId,
                            AssigneeId = user.Id,
                            DateAssigned = DateTime.UtcNow
                        };

                        assignees.Add(newAssign);
                    }

                    //Add the event to the database
                    db.EventsAssigneds.AddRange(assignees);

                    //Save the changes
                    await db.SaveChangesAsync();

                    //create new DTO
                    var calendarEventDTO = new ReturnCalendarEventDTO
                    {
                        Title = calendarEvent.Title,
                        Description = calendarEvent.Description,
                        StartDate = calendarEvent.StartDate.DateTime,
                        EndDate = calendarEvent.EndDate.DateTime,
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
                return StatusCode(500, ex.Message);
            }
        }

        //Update calendar event
        [HttpPut("calendar/{EventId}")]
        [Authorize]
        public async Task<IActionResult> UpdateCalendarEvent([FromQuery] Guid EventID, [FromBody] ReturnCalendarEventDTO model)
        {
            try
            {
                using (var db = contextFactory.CreateDbContext())
                {
                    //Get the calendar event
                    var events = await db.CalendarEvents.FirstOrDefaultAsync(u => u.EventId == model.EventId);

                    if (events == null)
                    {
                        return BadRequest("Event not found");
                    }

                    //update the calendar event
                    events.Title = model.Title;
                    events.Description = model.Description;
                    events.StartDate = model.StartDate;
                    events.EndDate = model.EndDate;
                    events.Color = model.Color;
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return BadRequest(ex.Message);

            }
            return Ok();
        }

        //delete calendar event
        [HttpDelete("calendar/{EventId}")]
        [Authorize]
        public async Task<IActionResult> DeleteCalendarEvent([FromQuery] Guid EventID, [FromBody] ReturnCalendarEventDTO model)
        {
            try
            {
                using (var db = contextFactory.CreateDbContext())
                {
                    //Get the calendar event
                    var events = await db.CalendarEvents.FirstOrDefaultAsync(u => u.EventId == model.EventId);

                    if (events == null)
                    {
                        return BadRequest("Event not found");
                    }

                    //delete the calendar event
                    db.CalendarEvents.Remove(events);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return BadRequest(ex.Message);

            }
            return Ok();
        }

        //display calendar events
        [HttpGet("calendar/{EventId}")]
        [Authorize]
        public async Task<IActionResult> GetCalendarEvent([FromQuery] Guid EventID, [FromBody] ReturnCalendarEventDTO model)
        {
            try
            {
                using (var db = contextFactory.CreateDbContext())
                {
                    //Get the calendar event
                    var events = await db.CalendarEvents.FirstOrDefaultAsync(u => u.EventId == model.EventId);

                    if (events == null)
                    {
                        return BadRequest("Event not found");
                    }
                    return Ok(events);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return BadRequest(ex.Message);

            }

        }
    }
}
