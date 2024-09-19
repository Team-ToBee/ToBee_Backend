using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using UserService.API.Models;
using System.Security.Claims;

namespace UserService.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;

		public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

		[HttpGet("signin-google")]
		public IActionResult SignInGoogle()
		{
			var redirectUrl = Url.Action("GoogleResponse", "Auth");
			var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
			return Challenge(properties, GoogleDefaults.AuthenticationScheme);
		}

		[HttpGet("signin-facebook")]
		public IActionResult SignInFacebook()
		{
			var redirectUrl = Url.Action("FacebookResponse", "Auth");
			var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
			return Challenge(properties, FacebookDefaults.AuthenticationScheme);
		}

		[HttpGet("google-response")]
		public async Task<IActionResult> GoogleResponse()
		{
			var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			if (result.Succeeded)
			{
				var externalUser = result.Principal;
				var email = externalUser?.FindFirstValue(ClaimTypes.Email);

				if (email == null)
				{
					return Redirect("/login-failed");
				}

				var user = await _userManager.FindByEmailAsync(email);

				if (user == null)
				{
					user = new ApplicationUser
					{
						UserName = email,
						Email = email
					};
					await _userManager.CreateAsync(user);

					if (result.Properties.Items.TryGetValue("LoginProvider", out var loginProvider) &&
						result.Properties.Items.TryGetValue("ProviderKey", out var providerKey) &&
						result.Properties.Items.TryGetValue("DisplayName", out var displayName))
					{
						var loginInfo = new UserLoginInfo(loginProvider, providerKey, displayName);
						await _userManager.AddLoginAsync(user, loginInfo);
					}
					else
					{
						return Redirect("/login-failed");
					}
				}

				await _signInManager.SignInAsync(user, isPersistent: false);
				return Redirect("/");
			}
			return Redirect("/login-failed");
		}

		[HttpGet("facebook-response")]
		public async Task<IActionResult> FacebookResponse()
		{
			var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			if (result.Succeeded)
			{
				var externalUser = result.Principal;
				var email = externalUser?.FindFirstValue(ClaimTypes.Email);

				if (email == null)
				{
					return Redirect("/login-failed");
				}

				var user = await _userManager.FindByEmailAsync(email);

				if (user == null)
				{
					user = new ApplicationUser
					{
						UserName = email,
						Email = email
					};
					await _userManager.CreateAsync(user);

					if (result.Properties.Items.TryGetValue("LoginProvider", out var loginProvider) &&
						result.Properties.Items.TryGetValue("ProviderKey", out var providerKey) &&
						result.Properties.Items.TryGetValue("DisplayName", out var displayName))
					{
						var loginInfo = new UserLoginInfo(loginProvider, providerKey, displayName);
						await _userManager.AddLoginAsync(user, loginInfo);
					}
					else
					{
						return Redirect("/login-failed");
					}
				}

				await _signInManager.SignInAsync(user, isPersistent: false);
				return Redirect("/");
			}
			return Redirect("/login-failed");
		}
	}
}
