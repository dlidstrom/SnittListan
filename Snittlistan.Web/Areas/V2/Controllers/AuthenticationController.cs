﻿namespace Snittlistan.Web.Areas.V2.Controllers
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Configuration;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Domain;
    using Indexes;
    using log4net;
    using Queue.Messages;
    using Raven.Abstractions;
    using Snittlistan.Web.Controllers;
    using Snittlistan.Web.Helpers;
    using Snittlistan.Web.HtmlHelpers;
    using Snittlistan.Web.Infrastructure.Attributes;
    using Snittlistan.Web.Services;

    public class AuthenticationController : AbstractController
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly Random Random = new Random();
        private readonly IAuthenticationService authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        [RestoreModelStateFromTempData]
        public ActionResult LogOn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(EmailViewModel vm, string returnUrl)
        {
            // find the user in question
            Models.User user = DocumentSession.FindUserByEmail(vm.Email);

            if (user == null)
            {
                // locate player
                Player[] players;
                if (vm.PlayerId != null)
                {
                    players = new[]
                    {
                        DocumentSession.Load<Player>(vm.PlayerId)
                    };
                }
                else
                {
                    players = DocumentSession.Query<Player, PlayerSearch>()
                                             .Where(x => x.Email == vm.Email
                                                         && (x.PlayerStatus == Player.Status.Active || x.PlayerStatus == Player.Status.Supporter))
                                             .ToArray();
                }

                if (players.Length == 0)
                {
                    ModelState.AddModelError("Email", "Spelare med den e-postadressen finns inte.");
                }
                else if (players.Length == 1)
                {
                    Player player = players[0];

                    // if player already has non-expired token, then reuse that one,
                    // else create a new token
                    OneTimeToken[] existingTokens =
                        DocumentSession.Query<OneTimeToken, OneTimeTokenIndex>()
                                       .Where(x => x.PlayerId == player.Id)
                                       .OrderByDescending(x => x.CreatedDate)
                                       .Take(10)
                                       .ToArray();
                    OneTimeToken validExistingToken = existingTokens.FirstOrDefault(x => x.IsExpired() == false);
                    if (validExistingToken != null)
                    {
                        // reuse still valid token
                        NotifyEvent($"{player.Name} - Samma token", validExistingToken.ToJson().ToString());
                        return RedirectToAction(
                            "LogOnOneTimePassword",
                            new { id = player.Id, validExistingToken.OneTimeKey, reuseToken = true });
                    }

                    // no valid token, generate new
                    var token = new OneTimeToken(player.Id);
                    Debug.Assert(Request.Url != null, "Request.Url != null");
                    string oneTimePassword =
                        string.Join("", Enumerable.Range(1, 6).Select(_ => Random.Next(10)));
                    token.Activate(
                        oneTimeKey =>
                        {
                            PublishMessage(new OneTimeKeyEvent(player.Email, oneTimePassword));
                        },
                        oneTimePassword);
                    NotifyEvent($"{player.Name} entered email address");
                    DocumentSession.Store(token);
                    return RedirectToAction(
                        "LogOnOneTimePassword",
                        new { id = player.Id, token.OneTimeKey });
                }
                else if (players.Length > 1)
                {
                    ViewBag.PlayerId = DocumentSession.CreatePlayerSelectList(
                        getPlayers: () => players,
                        textFormatter: p => $"{p.Name} ({p.Nickname})");
                    NotifyEvent($"{vm.Email} - Select from multiple {string.Join(", ", players.Select(x => $"{x.Name} ({x.Email})"))}");
                    return View();
                }
                else
                {
                    throw new Exception("Unhandled case");
                }
            }

            // redisplay form if any errors at this point
            if (ModelState.IsValid == false)
            {
                NotifyEvent($"{vm.Email} - ModelState invalid: {string.Join(", ", ModelState.Values.Select(x => string.Join(", ", x.Errors.Select(y => y.ErrorMessage))))}");
                return View(vm);
            }

            return RedirectToAction("LogOnPassword", new { vm.Email, returnUrl });
        }

        [RestoreModelStateFromTempData]
        public ActionResult LogOnOneTimePassword(string id, string oneTimeKey, bool? reuseToken)
        {
            DateTimeOffset? tokenDate = null;
            if (reuseToken ?? false)
            {
                OneTimeToken reusedToken = DocumentSession.Query<OneTimeToken, OneTimeTokenIndex>()
                    .SingleOrDefault(x => x.OneTimeKey == oneTimeKey);
                tokenDate = reusedToken?.Timestamp;
            }

            Player player = DocumentSession.Load<Player>(id);
            return View(new PasswordViewModel
            {
                Email = player.Email,
                RememberMe = true,
                OneTimeKey = oneTimeKey,
                ReuseToken = reuseToken ?? false,
                ReusedTokenDate = tokenDate
            });
        }

        [HttpPost]
        [SetTempModelState]
        public async Task<ActionResult> LogOnOneTimePassword(string id, PasswordViewModel vm)
        {
            if (Request.IsAuthenticated) return RedirectToAction("Index", "Roster");

            OneTimeToken[] activeTokens = DocumentSession.Query<OneTimeToken, OneTimeTokenIndex>()
                .Where(x => x.PlayerId == id && x.CreatedDate > SystemTime.UtcNow.ToLocalTime().AddDays(-1))
                .ToArray();
            Player player = DocumentSession.Load<Player>(id);
            if (player == null)
                throw new HttpException(404, "Player not found");

            try
            {
                if (activeTokens.Any() == false)
                {
                    Log.Info("No tokens");
                    ModelState.AddModelError("Lösenord", "Prova igen");
                    vm.Password = string.Empty;
                    await Task.Delay(2000);
                    NotifyEvent($"{player.Name} - Prova igen");
                    return View(vm);
                }

                OneTimeToken matchingPassword = activeTokens.FirstOrDefault(x => x.Payload == vm.Password);
                if (matchingPassword == null)
                {
                    Log.Info("No matching password token");
                    ModelState.AddModelError("Lösenord", "Felaktigt lösenord");
                    vm.Password = string.Empty;
                    await Task.Delay(2000);
                    NotifyEvent($"{player.Name} - Felaktig kod ({vm.Password})");
                    return View(vm);
                }

                authenticationService.SetAuthCookie(player.Id, vm.RememberMe);
                NotifyEvent($"{player.Name} logged in");
            }
            catch
            {
                //
            }

            return RedirectToAction("Index", "Roster");
        }

        public ActionResult LogOnPassword(string email, string returnUrl)
        {
            return View(new PasswordViewModel { Email = email, RememberMe = true });
        }

        [HttpPost]
        public ActionResult LogOnPassword(string returnUrl, PasswordViewModel vm)
        {
            // find the user in question
            Models.User user = DocumentSession.FindUserByEmail(vm.Email);

            if (!user.ValidatePassword(vm.Password))
            {
                ModelState.AddModelError("Password", "Lösenordet stämmer inte!");
            }
            else if (user.IsActive == false)
            {
                ModelState.AddModelError("Inactive", "Användaren har inte aktiverats");
            }

            // redisplay form if any errors at this point
            if (!ModelState.IsValid)
                return View(vm);

            Debug.Assert(user != null, "user != null");
            authenticationService.SetAuthCookie(user.Email, vm.RememberMe);

            if (Url.IsLocalUrl(returnUrl)
                && returnUrl.Length > 1
                && returnUrl.StartsWith("/")
                && !returnUrl.StartsWith("//")
                && !returnUrl.StartsWith("/\\"))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Roster");
        }

        public ActionResult LogOff()
        {
            authenticationService.SignOut();
            return RedirectToAction("Index", "Roster");
        }

        public class EmailViewModel
        {
            [Required(ErrorMessage = "Ange e-postadress")]
            [DataType(DataType.EmailAddress)]
            [Display(Name = "E-postadress")]
            public string Email { get; set; }

            public string PlayerId { get; set; }
        }

        public class PasswordViewModel
        {
            [Required(ErrorMessage = "Ange e-postadress")]
            [DataType(DataType.EmailAddress)]
            [Display(Name = "E-postadress")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Ange lösenord")]
            [DataType(DataType.Password)]
            [Display(Name = "Lösenord")]
            public string Password { get; set; }

            public bool RememberMe { get; set; }

            public string OneTimeKey { get; set; }

            public bool ReuseToken { get; set; }

            public DateTimeOffset? ReusedTokenDate { get; set; }
        }

        private void NotifyEvent(string subject, string body = null)
        {
            PublishMessage(
                EmailTask.Create(
                    ConfigurationManager.AppSettings["OwnerEmail"],
                    subject,
                    string.Join(
                        Environment.NewLine,
                        new[]
                        {
                            $"User Agent: {Request.UserAgent}",
                            $"Referrer: {Request.UrlReferrer}",
                            body ?? string.Empty
                        })));
        }
    }
}
