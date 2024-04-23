using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TeamTacticsBackend.Database;
using TeamTacticsBackend.DTO.Team;
using TeamTacticsBackend.Models.Teams;
using TeamTacticsBackend.Models.Users;

namespace TeamTacticsBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TeamsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IDbContextFactory<TeamTacticsDBContext> _contextFactory;

        public TeamsController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IDbContextFactory<TeamTacticsDBContext> contextFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _contextFactory = contextFactory;
        }

        [HttpGet("MyTeam")]
        [Authorize]
        public async Task<IActionResult> MyTeam()
        {
            try
            {
                var identityUser = await _userManager.GetUserAsync(User);

                if (identityUser == null)
                {
                    return Unauthorized();
                }

                using (var context = _contextFactory.CreateDbContext())
                {
                    var user = await context.Users.FirstOrDefaultAsync(u => u.Id == identityUser.Id);

                    if (user.TeamId == Guid.Empty)
                    {
                        return NotFound();
                    }

                    var team = await context.Teams.FirstOrDefaultAsync(t => t.TeamId == user.TeamId);

                    ReturnTeamDTO returnTeam = new ReturnTeamDTO
                    {
                        TeamId = team.TeamId,
                        TeamName = team.TeamName,
                        TeamSport = team.TeamSport,
                        TeamCity = team.TeamCity,
                        TeamState = team.TeamState,
                        TeamJoinCode = team.TeamJoinCode,
                        DateCreated = team.DateCreated
                    };

                    //get all athletes in the team
                    var teamMembers = await context.Users.Where(u => u.TeamId == team.TeamId).ToListAsync();

                    returnTeam.TeamMembers = teamMembers.Select(u => new TeamMemberDTO
                    {
                        userId = new Guid(u.Id),
                        userName = u.FirstName + " " + u.LastName,
                    }).ToList();

                    //foreach, get ident user and then get email
                    foreach (var member in returnTeam.TeamMembers)
                    {
                        var identUser = await _userManager.FindByIdAsync(member.userId.ToString());
                        member.email = identUser.Email;
                    }

                    return Ok(returnTeam);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return StatusCode(500);
            }
        }

        //Create\
        [HttpPost("CreateNewTeam")]
        [Authorize]
        public async Task<IActionResult> CreateNewTeam([FromBody]NewTeamDTO newTeam)
        {
            try
            {
                //Get the current user
                var identityUser = await _userManager.GetUserAsync(User);

                //Check if the user is signed in
                if (identityUser == null)
                {
                    return Unauthorized();
                }

                DateTimeOffset utcNow = DateTimeOffset.UtcNow;

                //Create a new team
                var team = new Team
                {
                    OwnerId = identityUser.Id,
                    TeamName = newTeam.TeamName,
                    TeamSport = newTeam.TeamSport,
                    TeamCity = newTeam.TeamCity,
                    TeamState = newTeam.TeamState,
                    DateCreated = utcNow
                };

                //Add the team to the database
                using (var context = _contextFactory.CreateDbContext())
                {
                    //generate team code. six digits letters and numbers
                    team.TeamJoinCode = Guid.NewGuid().ToString().Substring(0, 6);


                    //check across database that there is no match for the team code
                    while (await context.Teams.AnyAsync(t => t.TeamJoinCode == team.TeamJoinCode))
                    {
                        team.TeamJoinCode = Guid.NewGuid().ToString().Substring(0, 6);
                    }

                    context.Teams.Add(team);
                    await context.SaveChangesAsync();

                    //find the user
                    var user = await context.Users.FirstOrDefaultAsync(u => u.Id == identityUser.Id);

                    //add the team to the user
                    user.TeamId = team.TeamId;

                    //save the changes
                    await context.SaveChangesAsync();

                    //Return the team
                    return Ok(new ReturnTeamDTO
                    {
                        TeamId = team.TeamId,
                        OwnerName = user.FirstName + " " + user.LastName,
                        TeamName = team.TeamName,
                        TeamSport = team.TeamSport,
                        TeamCity = team.TeamCity,
                        TeamState = team.TeamState,
                        TeamJoinCode = team.TeamJoinCode,
                        DateCreated = team.DateCreated
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return StatusCode(500);
            }
        }

        //Read
        [HttpGet("GetTeam")]
        [Authorize]
        public async Task<IActionResult> GetTeam([FromQuery]Guid teamId)
        {
            try
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    var team = await context.Teams.FirstOrDefaultAsync(t => t.TeamId == teamId);

                    if (team == null)
                    {
                        return NotFound();
                    }

                    var user = await context.Users.FirstOrDefaultAsync(u => u.Id == team.OwnerId);

                    return Ok(new ReturnTeamDTO
                    {
                        TeamId = team.TeamId,
                        OwnerName = user.FirstName + " " + user.LastName,
                        TeamName = team.TeamName,
                        TeamSport = team.TeamSport,
                        TeamCity = team.TeamCity,
                        TeamState = team.TeamState,
                        TeamJoinCode = team.TeamJoinCode,
                        DateCreated = team.DateCreated
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return StatusCode(500);
            }
        }

        //Update
        [HttpPut("UpdateTeam")]
        [Authorize]
        public async Task<IActionResult> UpdateTeam([FromBody]ReturnTeamDTO updateTeam)
        {
            try
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    var team = await context.Teams.FirstOrDefaultAsync(t => t.TeamId == updateTeam.TeamId);

                    if (team == null)
                    {
                        return NotFound();
                    }

                    team.TeamName = updateTeam.TeamName;
                    team.TeamSport = updateTeam.TeamSport;
                    team.TeamCity = updateTeam.TeamCity;
                    team.TeamState = updateTeam.TeamState;

                    await context.SaveChangesAsync();

                    //return the updated team
                    ReturnTeamDTO returnTeam = new ReturnTeamDTO
                    {
                        TeamId = team.TeamId,
                        TeamName = team.TeamName,
                        TeamSport = team.TeamSport,
                        TeamCity = team.TeamCity,
                        TeamState = team.TeamState,
                        TeamJoinCode = team.TeamJoinCode,
                        DateCreated = team.DateCreated
                    };

                    //get all athletes in the team
                    var teamMembers = await context.Users.Where(u => u.TeamId == team.TeamId).ToListAsync();

                    returnTeam.TeamMembers = teamMembers.Select(u => new TeamMemberDTO
                    {
                        userId = new Guid(u.Id),
                        userName = u.FirstName + " " + u.LastName,
                    }).ToList();

                    //foreach, get ident user and then get email
                    foreach (var member in returnTeam.TeamMembers)
                    {
                        var identUser = await _userManager.FindByIdAsync(member.userId.ToString());
                        member.email = identUser.Email;
                    }

                    return Ok(returnTeam);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return StatusCode(500);
            }
        }

        //Delete
        [HttpDelete("DeleteTeam")]
        [Authorize]
        public async Task<IActionResult> DeleteTeam([FromQuery]Guid teamId)
        {
            try
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    var team = await context.Teams.FirstOrDefaultAsync(t => t.TeamId == teamId);

                    if (team == null)
                    {
                        return NotFound();
                    }

                    context.Teams.Remove(team);
                    await context.SaveChangesAsync();

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return StatusCode(500);
            }
        }

        //Join Team
        [HttpPost("JoinTeam")]
        [Authorize]
        public async Task<IActionResult> JoinTeam([FromQuery]Guid teamId)
        {
            try
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    var team = await context.Teams.FirstOrDefaultAsync(t => t.TeamId == teamId);

                    if (team == null)
                    {
                        return NotFound();
                    }

                    var identityUser = await _userManager.GetUserAsync(User);

                    if (identityUser == null)
                    {
                        return Unauthorized();
                    }

                    var user = await context.Users.FirstOrDefaultAsync(u => u.Id == identityUser.Id);

                    user.TeamId = teamId;

                    await context.SaveChangesAsync();

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return StatusCode(500);
            }
        }

        //Add User to Team
        [HttpPost("AddUserToTeam")]
        [Authorize]
        public async Task<IActionResult> AddUserToTeam([FromQuery]Guid teamId, [FromQuery]string userId)
        {
            try
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    var team = await context.Teams.FirstOrDefaultAsync(t => t.TeamId == teamId);

                    if (team == null)
                    {
                        return NotFound();
                    }

                    var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                    if (user == null)
                    {
                        return NotFound();
                    }

                    user.TeamId = teamId;

                    await context.SaveChangesAsync();

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return StatusCode(500);
            }
        }

        //Remove User from Team
        [HttpDelete("RemoveUserFromTeam")]
        [Authorize]
        public async Task<IActionResult> RemoveUserFromTeam([FromQuery]Guid teamId, [FromQuery]string userId)
        {
            try
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    var team = await context.Teams.FirstOrDefaultAsync(t => t.TeamId == teamId);

                    if (team == null)
                    {
                        return NotFound();
                    }

                    var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                    if (user == null)
                    {
                        return NotFound();
                    }

                    //ensure that user is not coach of the team
                    if (user.Id == team.OwnerId)
                    {
                        return BadRequest();
                    }

                    user.TeamId = null;

                    await context.SaveChangesAsync();

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return StatusCode(500);
            }
        }

        //Get Team Members
        [HttpGet("GetTeamMembers")]
        [Authorize]
        public async Task<IActionResult> GetTeamMembers([FromQuery]Guid teamId)
        {
            try
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    var team = await context.Teams.FirstOrDefaultAsync(t => t.TeamId == teamId);

                    if (team == null)
                    {
                        return NotFound();
                    }

                    var users = await context.Users.Where(u => u.TeamId == teamId).ToListAsync();

                    return Ok(users.Select(u => new
                    {
                        UserId = u.Id,
                        FirstName = u.FirstName,
                        LastName = u.LastName
                    }));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return StatusCode(500);
            }
        }

        //Generate Team Code
        [HttpGet("GenerateTeamCode")]
        [Authorize]
        public async Task<IActionResult> GenerateTeamCode([FromQuery]Guid teamId)
        {
            try
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    var team = await context.Teams.FirstOrDefaultAsync(t => t.TeamId == teamId);

                    if (team == null)
                    {
                        return NotFound();
                    }

                    team.TeamJoinCode = Guid.NewGuid().ToString().Substring(0, 6);

                    await context.SaveChangesAsync();

                    //return as json
                    return Ok(new
                    {
                        TeamJoinCode = team.TeamJoinCode
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return StatusCode(500);
            }
        }

        [HttpGet("GetTeamViaCode")]
        [Authorize]
        public async Task<IActionResult> GetTeamViaCode([FromQuery]string teamCode)
        {
            try
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    //find team code ignore case
                    var team = await context.Teams.FirstOrDefaultAsync(t => t.TeamJoinCode.ToLower() == teamCode.ToLower());

                    if (team == null)
                    {
                        return NotFound();
                    }

                    var user = await context.Users.FirstOrDefaultAsync(u => u.Id == team.OwnerId);

                    return Ok(new ReturnTeamDTO
                    {
                        TeamId = team.TeamId,
                        OwnerName = user.FirstName + " " + user.LastName,
                        TeamName = team.TeamName,
                        TeamSport = team.TeamSport,
                        TeamCity = team.TeamCity,
                        TeamState = team.TeamState,
                        TeamJoinCode = team.TeamJoinCode,
                        DateCreated = team.DateCreated
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return StatusCode(500);
            }
        }

       
    }

}
