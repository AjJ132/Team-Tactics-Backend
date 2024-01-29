using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Team_Tactics_Backend.Database;

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

    }

}
