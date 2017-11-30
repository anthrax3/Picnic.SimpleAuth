using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Picnic.Areas.Picnic.Controllers;
using Picnic.Options;
using Picnic.SimpleAuth.Model;
using Picnic.SimpleAuth.Service;

namespace Picnic.SimpleAuth.Areas.Picnic.Controllers
{
    [Route("")]
    [Area("Picnic")]
    public class AuthController : Controller
    {
        readonly IUserService UserService;

        /// <summary>
        /// ctor the Mighty
        /// </summary>
        public AuthController(IOptions<PicnicOptions> picnicOptionsProvider, IUserService userService)
        {
            this.UserService = userService;            
        }

        [HttpGet]
        [Route("login")]
        public async Task<IActionResult> Login()
        {
            var userCount = this.UserService.GetAllItems().Count;
            if (userCount == 0)
            {
                var user = new User { Id = "admin", Password = CryptoHelper.Crypto.HashPassword("password"), LastModifyDate = DateTime.Now, LastModifyUser = "System" };
                await this.UserService.SaveAsync(user);
            }

            return this.View(new LoginViewModel { ReturnUrl = Request.Query["ReturnUrl"] });            
        }

        [HttpPost]
        [Route("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = this.UserService.GetById(viewModel.Username);
                if (user != null)
                {
                    if (CryptoHelper.Crypto.VerifyHashedPassword(user.Password, viewModel.Password))
                    {
                        var identity = new ClaimsIdentity();
                        identity.AddClaim(new Claim(ClaimTypes.Name, user.Id));
                        identity.AddClaim(new Claim(ClaimTypes.Role, "PicnicUser"));

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(identity));

                        return Redirect(viewModel.ReturnUrl ?? Url.Action(nameof(RootController.Index), "Root"));
                    }
                }
            }

            return this.View(new LoginViewModel { Username = viewModel.Username, Password = null, IsLoginFailure = true });
        }

        [HttpGet]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect(Request.Query["ReturnUrl"].FirstOrDefault() ?? Url.Action(nameof(RootController.Index), "Root"));
        }
    }
}