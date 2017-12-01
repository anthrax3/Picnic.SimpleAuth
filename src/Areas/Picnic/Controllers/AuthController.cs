using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ctorx.Core.Mvc.Messaging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
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
        readonly IMessenger Messenger;

        /// <summary>
        /// ctor the Mighty
        /// </summary>
        public AuthController(IOptions<PicnicOptions> picnicOptionsProvider, IUserService userService, IMessenger messenger)
        {
            this.UserService = userService;
            this.Messenger = messenger;
        }

        [HttpGet]
        [Route("login")]
        public async Task<IActionResult> Login()
        {
            // Create a default user if no users exist
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
                        var identity = new ClaimsIdentity(new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.Id),
                            new Claim(ClaimTypes.Role, "PicnicUser")
                        });
                        
                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(identity));

                        return Redirect(viewModel.ReturnUrl ?? Url.Action(nameof(RootController.Index), "Root"));
                    }
                }
            }

            this.Messenger.AppendError("That username/password didn't work", "Sorry");

            return this.View(new LoginViewModel { Username = viewModel.Username, Password = null });
        }

        [HttpGet]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect(Request.Query["ReturnUrl"].FirstOrDefault() ?? Url.Action(nameof(RootController.Index), "Root"));
        }

        [HttpGet]
        [Route("change-password")]
        [Authorize("PicnicAuthPolicy")]
        public IActionResult ChangePassword()
        {
            return this.View();
        }

        [HttpPost]
        [Route("change-password")]
        [Authorize("PicnicAuthPolicy")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                this.Messenger.AppendError();
            }
            else
            {
                var user = this.UserService.GetById(User.Identity.Name);
                if (user != null)
                {
                    if (CryptoHelper.Crypto.VerifyHashedPassword(user.Password, viewModel.Current))
                    {
                        user.Password = CryptoHelper.Crypto.HashPassword(viewModel.New);
                        await this.UserService.SaveAsync(user);

                        this.Messenger.ForwardSuccess("Your password has been changed");
                        return RedirectToAction(nameof(ChangePassword));
                    }
                    else
                    {
                        this.Messenger.AppendError("The current password you entered is not correct");
                    }
                }
            }
            
            return this.View(new ChangePasswordViewModel());
        }
    }
}