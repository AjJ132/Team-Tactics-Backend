using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Team_Tactics_Backend.Database;
using Team_Tactics_Backend.Models.Users;

namespace Team_Tactics_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IDbContextFactory<TeamTacticsDBContext> _contextFactory;

        public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IDbContextFactory<TeamTacticsDBContext> contextFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _contextFactory = contextFactory;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                RegisterModel newUser = model;

                var user = new IdentityUser
                {
                    UserName = newUser.Email,
                    Email = newUser.Email,
                    EmailConfirmed = false
                };

                var result = await _userManager.CreateAsync(user, newUser.Password);

                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
                else
                {
                    //sign in user
                    await _signInManager.SignInAsync(user, false);

                    //Create new User object
                    User newUserModel = new User
                    {
                        Id = user.Id,
                        FirstName = newUser.FirstName,
                        LastName = newUser.LastName,
                        UserType = 0, //0 = Player, 1 = Coach, 2 = Admin
                        TeamId = Guid.Empty,
                        DateJoined = DateTime.Now
                    };

                    //Add new user to database
                    //create DB context using factory
                    using var context = _contextFactory.CreateDbContext();
                    context.Users.Add(newUserModel);
                    await context.SaveChangesAsync();

                    return Ok(user.Id);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

    }

}
