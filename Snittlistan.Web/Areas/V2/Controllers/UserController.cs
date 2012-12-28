﻿namespace Snittlistan.Web.Areas.V2.Controllers
{
    using System;
    using System.Web;
    using System.Web.Mvc;

    using JetBrains.Annotations;

    using Raven.Client;

    using Snittlistan.Web.Areas.V2.ViewModels;
    using Snittlistan.Web.Controllers;
    using Snittlistan.Web.Models;
    using Snittlistan.Web.Services;

    /// <summary>
    /// User administration.
    /// </summary>
    public class UserController : AdminController
    {
        private readonly IAuthenticationService authenticationService;

        /// <summary>
        /// Initializes a new instance of the UserController class.
        /// </summary>
        /// <param name="session">Document session.</param>
        /// <param name="authenticationService">Authentication service.</param>
        public UserController(IDocumentSession session, [NotNull] IAuthenticationService authenticationService)
            : base(session)
        {
            if (authenticationService == null) throw new ArgumentNullException("authenticationService");
            this.authenticationService = authenticationService;
        }

        public ActionResult SetPassword(string id, string activationKey)
        {
            var user = Session.Load<User>(id);
            if (user == null) throw new HttpException(404, "User not found");
            if (user.ActivationKey != activationKey) throw new InvalidOperationException("Unknown activation key");
            return this.View(new SetPasswordViewModel { ActivationKey = activationKey });
        }

        [HttpPost]
        public ActionResult SetPassword(string id, SetPasswordViewModel vm)
        {
            var user = Session.Load<User>(id);
            if (user == null) throw new HttpException(404, "User not found");
            if (ModelState.IsValid == false) return this.View(vm);
            user.SetPassword(vm.Password, vm.ActivationKey);
            authenticationService.SetAuthCookie(user.Email, true);
            return this.RedirectToAction("Index", "Roster");
        }
    }
}