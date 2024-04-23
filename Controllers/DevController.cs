using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TeamTacticsBackend.Database;
using TeamTacticsBackend.Models.Users;

namespace TeamTacticsBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DevControler : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IDbContextFactory<TeamTacticsDBContext> _contextFactory;

        public DevControler(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IDbContextFactory<TeamTacticsDBContext> contextFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _contextFactory = contextFactory;
        }

        [HttpPost("dev-delete-all")]
        public async Task<IActionResult> DevDeleteAll()
        {
            try
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                }
                return Ok("Database deleted and recreated");

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

    }

}
