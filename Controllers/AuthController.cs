using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TeamTacticsBackend.Database;
using TeamTacticsBackend.DTO;
using TeamTacticsBackend.Models.Users;

namespace TeamTacticsBackend.Controllers
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

                    DateTime utcnow = DateTime.UtcNow;

                    //Create new User object
                    User newUserModel = new User
                    {
                        Id = user.Id,
                        FirstName = newUser.FirstName,
                        LastName = newUser.LastName,
                        UserType = 0, //0 = Player, 1 = Coach, 2 = Admin
                        TeamId = Guid.Empty,
                        DateJoined = utcnow,
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

        [HttpPost("signin")]
        public async Task<IActionResult> Signin([FromBody] LoginModel model)
        {
            try
            {
                //verify model
                if (model.Email == null || model.Password == null)
                {
                    return BadRequest("Invalid model");
                }

                //sign in user
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

                if (result.Succeeded)
                {
                    //return user class
                    var _context = _contextFactory.CreateDbContext();
                    var identUser = await _userManager.FindByEmailAsync(model.Email);
                    User user = _context.Users.FirstOrDefault(u => u.Id == identUser.Id);

                    UserInfoReturnModel userReturn = new UserInfoReturnModel
                    {
                        Email = identUser.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Id = user.Id
                    };

                    return Ok(userReturn);
                }
                else
                {
                    return BadRequest("Incorrect email or password");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("check-authentication")]
        public async Task<IActionResult> CheckAuthentication()
        {
            try
            {
                //check if user is signed in
                if (User.Identity.IsAuthenticated)
                {
                    //return user class
                    var _context = _contextFactory.CreateDbContext();
                    var identUser = await _userManager.FindByEmailAsync(User.Identity.Name);
                    var user = _context.Users.FirstOrDefault(u => u.Id == identUser.Id);

                    UserInfoReturnModel userReturn = new UserInfoReturnModel
                    {
                        Email = identUser.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Id = user.Id
                    };

                    return Ok(userReturn);
                }
                else
                {
                    return BadRequest("User not signed in");
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
