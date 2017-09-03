using System.Linq;
using System.Threading.Tasks;
using CustomIdentity20.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CustomIdentity20.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Route("login")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody]LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return Ok(new { LoginSuccess = true });
                }
            }

            return BadRequest("Invalid login attempt!");
        }

        [Route("register")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody]LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return Ok(new { LoginSuccess = true });
                }

            }

            return BadRequest("Invalid login attempt!");
        }

        [Route("logout")]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { Success = true });
        }

        [Authorize]
        [Route("test")]
        [HttpGet]
        public ActionResult Test()
        {
            return Ok(User.Claims.Select(m => new
            {
                m.Type,
                m.Value
            }).ToList());
        }

    }
}
