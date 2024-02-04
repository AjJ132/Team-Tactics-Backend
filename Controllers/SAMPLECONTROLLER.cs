// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore.Internal;
// using Microsoft.EntityFrameworkCore;
// using TeamTactics_ClubCore_Server_API.Database;
// using TeamTactics_ClubCore_Server_API.Models.Util;
// using TeamTactics_ClubCore_Server_API.Models;
// using System.Diagnostics;
// using Microsoft.AspNetCore.Authorization;

// namespace TeamTactics_ClubCore_Server_API.Controllers
// {
//     [ApiController]
//     [Route("[controller]")]
//     public class DevController : ControllerBase
//     {
//         private readonly UserManager<IdentityUser> _userManager;
//         private readonly IDbContextFactory<TeamTacticsDbContext> _contextFactory;
//         public DevController(UserManager<IdentityUser> userManager, IDbContextFactory<TeamTacticsDbContext> contextFactory)
//         {
//             _userManager = userManager;
//             _contextFactory = contextFactory;
//         }

//         [HttpGet("generate-test-data")] //pre load the database with test data
//         public async Task<IActionResult> GenerateTestData()
//         {
//             try
//             {
//                 //first delete all data in the database
//                 using (var context = _contextFactory.CreateDbContext())
//                 {
//                     //context.Database.ExecuteSqlRaw("TRUNCATE TABLE\r\n  \"AspNetRoleClaims\",\r\n  \"AspNetRoles\",\r\n  \"AspNetUserClaims\",\r\n  \"AspNetUserLogins\",\r\n  \"AspNetUserRoles\",\r\n  \"AspNetUserTokens\",\r\n  \"AspNetUsers\",\r\n  \"CalendarEvents\",\r\n  \"ConversationMessages\",\r\n  \"ConversationUsers\",\r\n  \"DirectConversations\",\r\n  \"DirectMessages\",\r\n  \"GroupConversations\",\r\n  \"Teams\"\r\nCASCADE;\r\n");
//                     await context.Database.EnsureDeletedAsync();

//                     //then ensure created
//                     await context.Database.EnsureCreatedAsync();

//                     //create test users
//                     var createdASPUsers = await CreateASPTestUsers();

//                     if (!createdASPUsers)
//                     {
//                         return StatusCode(500, "Error creating ASP Users");
//                     }

//                     //create users
//                     var createdUsers2 = await CreateUsers();

//                     if (!createdUsers2)
//                     {
//                         return StatusCode(500, "Error creating Users");
//                     }

//                     //create teams
//                     var createdTeams = await CreateTeams();

//                     if (!createdTeams)
//                     {
//                         return StatusCode(500, "Error creating Teams");
//                     }

//                     //create calendar events
//                     var createdCalendarEvents = await CreateCalendarEvents();

//                     if (!createdCalendarEvents)
//                     {
//                         return StatusCode(500, "Error creating Calendar Events");
//                     }

//                     //create messages
//                     var createdMessages = await CreateMessages();

//                     if (!createdMessages)
//                     {
//                         return StatusCode(500, "Error creating Messages");
//                     }

//                     return Ok("Test data created successfully");
//                 }


//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, "Internal Server Error");
//             }
//         }

//         private async Task<bool> CreateASPTestUsers()
//         {
//             try
//             {
//                 var identUser1 = new IdentityUser
//                 {
//                     UserName = "aj132@icloud.com",
//                     Email = "aj132@icloud.com"
//                 };
//                 var result1 = await _userManager.CreateAsync(identUser1, "Test123$");

//                 var identUser2 = new IdentityUser
//                 {
//                     UserName = "sport2848@gmail.com",
//                     Email = "sport2848@gmail.com"
//                 };
//                 var result2 = await _userManager.CreateAsync(identUser2, "Test123$");

//                 var identUser3 = new IdentityUser
//                 {
//                     UserName = "testuser2002@gmail.com",
//                     Email = "testuser2002@gmail.com"
//                 };
//                 var result3 = await _userManager.CreateAsync(identUser3, "Test123$");

//                 return result1.Succeeded && result2.Succeeded && result3.Succeeded;
//             }
//             catch (Exception ex)
//             {
//                 Debug.WriteLine(ex.Message);
//                 return false;
//             }
//         }

//         private async Task<bool> CreateUsers()
//         {
//             try
//             {
//                 using(var _context = await _contextFactory.CreateDbContextAsync())
//                 {
//                     var user1 = await _userManager.FindByEmailAsync("aj132@icloud.com");
//                     var user2 = await _userManager.FindByEmailAsync("sport2848@gmail.com");
//                     var user3 = await _userManager.FindByEmailAsync("testuser2002@gmail.com");

//                     Users users1 = new Users()
//                     {
//                         User_ID = user1.Id,
//                         First_Name = "AJ",
//                         Last_Name = "Johnson",
//                         User_Type = 1,
//                         Date_Joined = DateTimeOffset.UtcNow
//                     };

//                     Users users2 = new Users()
//                     {
//                         User_ID = user2.Id,
//                         First_Name = "Sport",
//                         Last_Name = "Brown",
//                         User_Type = 1,
//                         Date_Joined = DateTimeOffset.UtcNow
//                     };

//                     Users users3 = new Users()
//                     {
//                         User_ID = user3.Id,
//                         First_Name = "John",
//                         Last_Name = "Doe",
//                         User_Type = 1,
//                         Date_Joined = DateTimeOffset.UtcNow
//                     };

//                     _context.Users.Add(users1);
//                     _context.Users.Add(users2);
//                     _context.Users.Add(users3);
//                     await _context.SaveChangesAsync();

//                     return true;

//                 }
//             }
//             catch (Exception ex)
//             {
//                 Debug.WriteLine(ex.Message);
//                 return false;
//             }
//         }

//         private async Task<bool> CreateTeams()
//         {
//             try
//             {
//                 using (var _context = await _contextFactory.CreateDbContextAsync())
//                 {
//                     var user1 = await _userManager.FindByEmailAsync("aj132@icloud.com");
//                     var user2 = await _userManager.FindByEmailAsync("sport2848@gmail.com");
//                     var user3 = await _userManager.FindByEmailAsync("testuser2002@gmail.com");

//                     Teams team1 = new Teams()
//                     {
//                         Team_ID = Guid.NewGuid(),
//                         Owner_ID = user3.Id,
//                         Team_Name = "KSU Owls",
//                         Team_Sport = "Pole Vault",
//                         Team_City = "Kennesaw",
//                         Team_State = "GA",
//                         Team_Join_Code = "ABC123",
//                         User_Count = 3,
//                         Team_Date_Joined = DateTimeOffset.UtcNow.AddYears(-3)
//                     };

//                     //Add team to database
//                     _context.Teams.Add(team1);
//                     await _context.SaveChangesAsync();

//                     //Add users to team
//                     var users1 = await _context.Users.Where(u => u.User_ID == user1.Id).FirstOrDefaultAsync();
//                     var users2 = await _context.Users.Where(u => u.User_ID == user2.Id).FirstOrDefaultAsync();
//                     var users3 = await _context.Users.Where(u => u.User_ID == user3.Id).FirstOrDefaultAsync();

//                     users1.Team_ID = team1.Team_ID;
//                     users2.Team_ID = team1.Team_ID;
//                     users3.Team_ID = team1.Team_ID;
//                     users3.User_Type = 3; //3 for coach, 2 for assistant coach, 1 for athlete

//                     _context.Users.Update(users1);
//                     _context.Users.Update(users2);
//                     _context.Users.Update(users3);

//                     await _context.SaveChangesAsync();

//                     return true;
//                 }
//             }
//             catch (Exception ex)
//             {
//                 Debug.WriteLine(ex.Message);
//                 return false;
//             }
//         }

//         private async Task<bool> CreateCalendarEvents()
//         {
//             return true; //TODO implement
//         }

//         private async Task<bool> CreateMessages()
//         {
//             try
//             {
//                 return true; //TODO implement
//             }
//             catch
//             {
//                 return false;
//             }
//         }



//         [HttpPost("test-authentication")]
//         [Authorize]
//         public async Task<IActionResult> TestAuthentication()
//         {
//             try
//             {
//                 var identUser = await _userManager.GetUserAsync(User);

//                 if (identUser == null)
//                 {
//                     return StatusCode(401, "IdentUser Not Present");
//                 }

//                 var dbUser = await _contextFactory.CreateDbContext().Users.Where(u => u.User_ID == identUser.Id).FirstOrDefaultAsync(); 

//                 if (dbUser == null)
//                 {
//                     return StatusCode(401, "DBUser Not Present");
//                 }

//                 return Ok("Authentication Successful");
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, "Internal Server Error");
//             }
//         }

//         [HttpPost("remove-all-message-data")]
//         public async Task<IActionResult> RemoveAllMessageData()
//         {
//             try
//             {
//                 using (var _context = await _contextFactory.CreateDbContextAsync())
//                 {
//                     _context.Database.ExecuteSqlRaw("TRUNCATE TABLE\r\n  \"GroupConversationMessages\",\r\n  \"ConversationUsers\",\r\n  \"DirectConversations\",\r\n  \"DirectMessages\",\r\n  \"GroupConversations\",\r\n  \"GroupConversationUserSeenStatuses\"\r\nCASCADE;\r\n");
//                     await _context.SaveChangesAsync();

//                     return Ok("All message data removed");
//                 }
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, ex.Message);
//             }
//         }
//     }
// }
