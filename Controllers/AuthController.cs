using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TeamTacticsBackend.Database;
using TeamTacticsBackend.DTO;
using TeamTacticsBackend.Models.Users;
using System.Text.RegularExpressions;


namespace TeamTacticsBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IDbContextFactory<TeamTacticsDBContext> _contextFactory;

        public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IDbContextFactory<TeamTacticsDBContext> contextFactory, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _contextFactory = contextFactory;
            _roleManager = roleManager;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                //verify model
                if (model.Email == null || model.Password == null || model.FirstName == null || model.LastName == null || model.Role == null)
                {
                    return BadRequest("Invalid model");
                }

                //ensures fields arent empty
                if (model.Email == "" || model.Password == "" || model.FirstName == "" || model.LastName == "" || model.Role == "")
                {
                    return BadRequest("Invalid model");
                }

                //regex email
                if (!Regex.IsMatch(model.Email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
                {
                    return BadRequest("Invalid email");
                }

                //regex password for uppercase, lowercase, number, and special character and length of 8
                if (!Regex.IsMatch(model.Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$"))
                {
                    return BadRequest("Invalid password");
                }

                //verify role exists using role manager
                if (!await _roleManager.RoleExistsAsync(model.Role))
                {
                    return BadRequest("Role does not exist");
                }

                //create user
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    //add role to user
                    await _userManager.AddToRoleAsync(user, model.Role);

                    //create user in user table
                    var _context = _contextFactory.CreateDbContext();
                    User newUser = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Id = user.Id
                    };

                    _context.Users.Add(newUser);
                    await _context.SaveChangesAsync();

                    //return user object
                    UserInfoReturnModel userReturn = new UserInfoReturnModel
                    {
                        Email = user.Email,
                        FirstName = newUser.FirstName,
                        LastName = newUser.LastName,
                        Id = newUser.Id,
                        role = model.Role
                    };

                    //sign the user in
                    await _signInManager.SignInAsync(user, false);

                    return Ok(userReturn);
                }
                else
                {
                    var error = result.Errors;
                    return BadRequest(error);
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
                    var role = await _userManager.GetRolesAsync(identUser);

                    UserInfoReturnModel userReturn = new UserInfoReturnModel
                    {
                        Email = identUser.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Id = user.Id,
                        role = role[0]
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
                    var role = await _userManager.GetRolesAsync(identUser);
                    
                    Guid? teamId = user.TeamId;




                    UserInfoReturnModel userReturn = new UserInfoReturnModel
                    {
                        Email = identUser.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Id = user.Id,
                        role = role[0],
                        teamId = teamId
                    };

                    //sign the user in
                    await _signInManager.SignInAsync(identUser, false);

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

        [HttpPost("signout")]
        public async Task<IActionResult> Signout()
        {
            try
            {
                //sign out user
                await _signInManager.SignOutAsync();
                return Ok("User signed out");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }

}
