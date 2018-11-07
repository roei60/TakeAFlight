using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TakeAFlight.Data;
using TakeAFlight.Models;
using TakeAFlight.Models.ManageViewModels;
using TakeAFlight.Services;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TakeAFlight.Controllers
{
	[Authorize]
	[Route("[controller]/[action]")]
	public class ManageController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IEmailSender _emailSender;
		private readonly ILogger _logger;
		private readonly UrlEncoder _urlEncoder;
		private readonly TakeAFlightContext _takeAFlightContext;
		private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
		private const string RecoveryCodesKey = nameof(RecoveryCodesKey);

		public ManageController(
		  UserManager<ApplicationUser> userManager,
		  SignInManager<ApplicationUser> signInManager,
		  IEmailSender emailSender,
		  ILogger<ManageController> logger,
		  UrlEncoder urlEncoder, TakeAFlightContext dbContext)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_emailSender = emailSender;
			_logger = logger;
			_urlEncoder = urlEncoder;
			_takeAFlightContext = dbContext;
		}

		private void SetListData()
		{
			Sex gender = new Sex();
			Nationality nationality = new Nationality();
			ViewBag.NationalityList = nationality.ToSelectList();
			ViewBag.SexList = gender.ToSelectList();

		}

		[TempData]
		public string StatusMessage { get; set; }

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			SetListData();
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}
			var passenger = _takeAFlightContext.Passengers.Include(p => p.User).SingleOrDefault(u => u.ApplicationUserID == user.Id);
			//if (passenger == null)
			//{
			//	return NotFound();
			//}

			var model = new IndexViewModel
			{
				Username = user.UserName,
				Email = user.Email,
				//PhoneNumber = user.PhoneNumber,
				IsEmailConfirmed = user.EmailConfirmed,
				StatusMessage = StatusMessage,
				Passenger = passenger
			};

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Index(IndexViewModel model)
		{
			SetListData();

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var email = user.Email;
			if (model.Email != email)
			{
				var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
				if (!setEmailResult.Succeeded)
				{
					throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
				}
			}
			var Passenger = _takeAFlightContext.Passengers.Include(u=>u.User).FirstOrDefault(obj => obj.ApplicationUserID == user.Id);

			if(Passenger!=null)
			{
				model.Passenger.ApplicationUserID = Passenger.ApplicationUserID;
				model.Passenger.User = Passenger.User;
				model.Passenger.ID = Passenger.ID;
				_takeAFlightContext.Entry(Passenger).State = EntityState.Detached;
				_takeAFlightContext.Update(model.Passenger);
				await _takeAFlightContext.SaveChangesAsync();
			}
			else
			{
				StatusMessage = "Error: Cannot find passenger data... pls try again later ";
				return RedirectToAction(nameof(Index));

			}

			//var phoneNumber = user.PhoneNumber;
			//if (model.PhoneNumber != phoneNumber)
			//{
			//	var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
			//	if (!setPhoneResult.Succeeded)
			//	{
			//		throw new ApplicationException($"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
			//	}
			//}

			StatusMessage = "Your profile has been updated";
			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SendVerificationEmail(IndexViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
			var email = user.Email;
			await _emailSender.SendEmailConfirmationAsync(email, callbackUrl);

			StatusMessage = "Verification email sent. Please check your email.";
			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public async Task<IActionResult> ChangePassword()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var hasPassword = await _userManager.HasPasswordAsync(user);
			if (!hasPassword)
			{
				return RedirectToAction(nameof(SetPassword));
			}

			var model = new ChangePasswordViewModel { StatusMessage = StatusMessage };
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
			if (!changePasswordResult.Succeeded)
			{
				AddErrors(changePasswordResult);
				return View(model);
			}

			await _signInManager.SignInAsync(user, isPersistent: false);
			_logger.LogInformation("User changed their password successfully.");
			StatusMessage = "Your password has been changed.";

			return RedirectToAction(nameof(ChangePassword));
		}

		[HttpGet]
		public async Task<IActionResult> SetPassword()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var hasPassword = await _userManager.HasPasswordAsync(user);

			if (hasPassword)
			{
				return RedirectToAction(nameof(ChangePassword));
			}

			var model = new SetPasswordViewModel { StatusMessage = StatusMessage };
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
			if (!addPasswordResult.Succeeded)
			{
				AddErrors(addPasswordResult);
				return View(model);
			}

			await _signInManager.SignInAsync(user, isPersistent: false);
			StatusMessage = "Your password has been set.";

			return RedirectToAction(nameof(SetPassword));
		}

		[HttpGet]
		public async Task<IActionResult> ExternalLogins()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var model = new ExternalLoginsViewModel { CurrentLogins = await _userManager.GetLoginsAsync(user) };
			model.OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
				.Where(auth => model.CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
				.ToList();
			model.ShowRemoveButton = await _userManager.HasPasswordAsync(user) || model.CurrentLogins.Count > 1;
			model.StatusMessage = StatusMessage;

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> LinkLogin(string provider)
		{
			// Clear the existing external cookie to ensure a clean login process
			await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

			// Request a redirect to the external login provider to link a login for the current user
			var redirectUrl = Url.Action(nameof(LinkLoginCallback));
			var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
			return new ChallengeResult(provider, properties);
		}

		[HttpGet]
		public async Task<IActionResult> LinkLoginCallback()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var info = await _signInManager.GetExternalLoginInfoAsync(user.Id);
			if (info == null)
			{
				throw new ApplicationException($"Unexpected error occurred loading external login info for user with ID '{user.Id}'.");
			}

			var result = await _userManager.AddLoginAsync(user, info);
			if (!result.Succeeded)
			{
				throw new ApplicationException($"Unexpected error occurred adding external login for user with ID '{user.Id}'.");
			}

			// Clear the existing external cookie to ensure a clean login process
			await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

			StatusMessage = "The external login was added.";
			return RedirectToAction(nameof(ExternalLogins));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> RemoveLogin(RemoveLoginViewModel model)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var result = await _userManager.RemoveLoginAsync(user, model.LoginProvider, model.ProviderKey);
			if (!result.Succeeded)
			{
				throw new ApplicationException($"Unexpected error occurred removing external login for user with ID '{user.Id}'.");
			}

			await _signInManager.SignInAsync(user, isPersistent: false);
			StatusMessage = "The external login was removed.";
			return RedirectToAction(nameof(ExternalLogins));
		}

		[HttpGet]
		public async Task<IActionResult> TwoFactorAuthentication()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var model = new TwoFactorAuthenticationViewModel
			{
				HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
				Is2faEnabled = user.TwoFactorEnabled,
				RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user),
			};

			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> Disable2faWarning()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			if (!user.TwoFactorEnabled)
			{
				throw new ApplicationException($"Unexpected error occured disabling 2FA for user with ID '{user.Id}'.");
			}

			return View(nameof(Disable2fa));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Disable2fa()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
			if (!disable2faResult.Succeeded)
			{
				throw new ApplicationException($"Unexpected error occured disabling 2FA for user with ID '{user.Id}'.");
			}

			_logger.LogInformation("User with ID {UserId} has disabled 2fa.", user.Id);
			return RedirectToAction(nameof(TwoFactorAuthentication));
		}

		[HttpGet]
		public async Task<IActionResult> EnableAuthenticator()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var model = new EnableAuthenticatorViewModel();
			await LoadSharedKeyAndQrCodeUriAsync(user, model);

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel model)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			if (!ModelState.IsValid)
			{
				await LoadSharedKeyAndQrCodeUriAsync(user, model);
				return View(model);
			}

			// Strip spaces and hypens
			var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

			var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
				user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

			if (!is2faTokenValid)
			{
				ModelState.AddModelError("Code", "Verification code is invalid.");
				await LoadSharedKeyAndQrCodeUriAsync(user, model);
				return View(model);
			}

			await _userManager.SetTwoFactorEnabledAsync(user, true);
			_logger.LogInformation("User with ID {UserId} has enabled 2FA with an authenticator app.", user.Id);
			var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
			TempData[RecoveryCodesKey] = recoveryCodes.ToArray();

			return RedirectToAction(nameof(ShowRecoveryCodes));
		}

		[HttpGet]
		public IActionResult ShowRecoveryCodes()
		{
			var recoveryCodes = (string[])TempData[RecoveryCodesKey];
			if (recoveryCodes == null)
			{
				return RedirectToAction(nameof(TwoFactorAuthentication));
			}

			var model = new ShowRecoveryCodesViewModel { RecoveryCodes = recoveryCodes };
			return View(model);
		}

		[HttpGet]
		public IActionResult ResetAuthenticatorWarning()
		{
			return View(nameof(ResetAuthenticator));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResetAuthenticator()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			await _userManager.SetTwoFactorEnabledAsync(user, false);
			await _userManager.ResetAuthenticatorKeyAsync(user);
			_logger.LogInformation("User with id '{UserId}' has reset their authentication app key.", user.Id);

			return RedirectToAction(nameof(EnableAuthenticator));
		}

		[HttpGet]
		public async Task<IActionResult> GenerateRecoveryCodesWarning()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			if (!user.TwoFactorEnabled)
			{
				throw new ApplicationException($"Cannot generate recovery codes for user with ID '{user.Id}' because they do not have 2FA enabled.");
			}

			return View(nameof(GenerateRecoveryCodes));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> GenerateRecoveryCodes()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			if (!user.TwoFactorEnabled)
			{
				throw new ApplicationException($"Cannot generate recovery codes for user with ID '{user.Id}' as they do not have 2FA enabled.");
			}

			var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
			_logger.LogInformation("User with ID {UserId} has generated new 2FA recovery codes.", user.Id);

			var model = new ShowRecoveryCodesViewModel { RecoveryCodes = recoveryCodes.ToArray() };

			return View(nameof(ShowRecoveryCodes), model);
		}

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyOrders(DateTime Departure, int DestId = -1, string sortExpression = "Flight.Destination", int page = 1, float Price = float.MaxValue)
        {
            IQueryable<FlightOrder> orders;
            var user = await _userManager.GetUserAsync(User);
            var passanger = _takeAFlightContext.Passengers.FirstOrDefault(obj => obj.ApplicationUserID == user.Id);

            var allFlightOrders = _takeAFlightContext.FlightOrders.Include(obj => obj.Flight).Include(obj => obj.Flight.Destination);

            if (DestId == -1)
            {
                orders = from flightOrder in allFlightOrders
                             where flightOrder.PassengerID == passanger.ID && flightOrder.Flight.Price <= Price && flightOrder.Flight.Departure > Departure
                             select flightOrder;
            }
            else
            {
                orders = from flightOrder in allFlightOrders
                         where flightOrder.PassengerID == passanger.ID && flightOrder.Flight.Price <= Price && flightOrder.Flight.Departure > Departure && flightOrder.Flight.DestinationID == DestId
                         select flightOrder;
            }

            var model = await PagingList.CreateAsync(orders, 10, page, sortExpression, "Flight.Destination");
            model.RouteValue = new RouteValueDictionary { { "DestId", DestId }, { "Price", Price }, { "Departure", Departure } };
            model.Action = "MyOrders";

            ViewBag.Items = _takeAFlightContext.Destinations.Select(obj => new SelectListItem()
            {
                Text = obj.ToString(),
                Value = obj.DestinationID.ToString()
            }).ToList();

            return View(model);
        }


        #region Destination

        [Authorize(Roles = "Admin")]
		public async Task<IActionResult> ViewDestinations(string sortExpression = "Country", int page = 1, string filter = "")
		{
			var Dest = from dest in _takeAFlightContext.Destinations
					   select dest;
			int pageSize = 10;
			if (!string.IsNullOrWhiteSpace(filter))
			{
				Dest = Dest.Where(p => p.Country.Contains(filter) || p.City.Contains(filter));
			}

			var model = await PagingList.CreateAsync(Dest, pageSize, page, sortExpression, "Country");
			model.Action = "ViewDestinations";

			model.RouteValue = new RouteValueDictionary { { "filter", filter } };
			return View(model);
		}
		[Authorize(Roles = "Admin")]
		public IActionResult CreateDestination()
		{
			return View();
		}
		[Authorize(Roles = "Admin")]

		public async Task<IActionResult> DeleteDestination(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var destination = await _takeAFlightContext.Destinations.SingleOrDefaultAsync(m => m.DestinationID== id);
			if (destination== null)
			{
				return NotFound();
			}

			return View(destination);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteDestinationConfirmed(int DestinationID)
		{
			var destination = await _takeAFlightContext.Destinations.SingleOrDefaultAsync(m => m.DestinationID== DestinationID);
			if (destination != null)
			{
				_takeAFlightContext.Destinations.Remove(destination);
				await _takeAFlightContext.SaveChangesAsync();
			}
			return RedirectToAction(nameof(ViewDestinations));
		}

		// POST: Flights/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> CreateDestination([Bind("DestinationID,Country,City")] Destination destination)
		{
			if (ModelState.IsValid)
			{
				_takeAFlightContext.Add(destination);
				await _takeAFlightContext.SaveChangesAsync();
				return RedirectToAction(nameof(ViewDestinations));
			}
			return View(destination);
		}


		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> EditDestination(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var destination= await _takeAFlightContext.Destinations.SingleOrDefaultAsync(m => m.DestinationID == id);
			if (destination == null)
			{
				return NotFound();
			}
			return View(destination);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int DestinationID, [Bind("DestinationID,Country,City")] Destination destination)
		{
			if (DestinationID != destination.DestinationID)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_takeAFlightContext.Update(destination);
					await _takeAFlightContext.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!_takeAFlightContext.Destinations.Any(obj=>obj.DestinationID==DestinationID))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(ViewDestinations));
			}
			return View(destination);
		}
		#endregion

		#region Helpers

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}
		}

		private string FormatKey(string unformattedKey)
		{
			var result = new StringBuilder();
			int currentPosition = 0;
			while (currentPosition + 4 < unformattedKey.Length)
			{
				result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
				currentPosition += 4;
			}
			if (currentPosition < unformattedKey.Length)
			{
				result.Append(unformattedKey.Substring(currentPosition));
			}

			return result.ToString().ToLowerInvariant();
		}

		private string GenerateQrCodeUri(string email, string unformattedKey)
		{
			return string.Format(
				AuthenticatorUriFormat,
				_urlEncoder.Encode("TakeAFlight"),
				_urlEncoder.Encode(email),
				unformattedKey);
		}

		private async Task LoadSharedKeyAndQrCodeUriAsync(ApplicationUser user, EnableAuthenticatorViewModel model)
		{
			var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
			if (string.IsNullOrEmpty(unformattedKey))
			{
				await _userManager.ResetAuthenticatorKeyAsync(user);
				unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
			}

			model.SharedKey = FormatKey(unformattedKey);
			model.AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey);
		}

		#endregion
	}
}
