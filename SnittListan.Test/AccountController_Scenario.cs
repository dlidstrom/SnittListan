﻿using System;
using System.Linq;
using Moq;
using MvcContrib.TestHelper;
using SnittListan.Controllers;
using SnittListan.Events;
using SnittListan.Helpers;
using SnittListan.Models;
using SnittListan.Services;
using SnittListan.ViewModels;
using Xunit;

namespace SnittListan.Test
{
	public class AccountController_Scenario : DbTest
	{
		[Fact]
		public void CanLogOnAfterRegisteringAndVerifyingAccount()
		{
			// register
			var model = new RegisterViewModel
				{
					FirstName = "F",
					LastName = "L",
					Email = "e@d.com",
					ConfirmEmail = "e@d.com",
					Password = "some pwd"
				};

			var controller1 = new AccountController(Session, Mock.Of<IFormsAuthenticationService>());
			using (DomainEvent.Disable())
				controller1.Register(model);

			// let indexing do its job
			WaitForNonStaleResultsAsOfLastWrite<User>();

			// verify
			var registeredUser = Session.FindUserByEmail("e@d.com").SingleOrDefault();
			registeredUser.ShouldNotBeNull("Should find user after registration");
			var key = registeredUser.ActivationKey;

			var controller2 = new AccountController(Session, Mock.Of<IFormsAuthenticationService>());
			controller2.Verify(Guid.Parse(key));

			// let indexing do its job
			WaitForNonStaleResultsAsOfLastWrite<User>();

			// logon
			bool loggedOn = false;
			var service = Mock.Of<IFormsAuthenticationService>();
			Mock.Get(service)
				.Setup(s => s.SetAuthCookie(It.Is<string>(e => e == "e@d.com"), It.IsAny<bool>()))
				.Callback(() => loggedOn = true);

			var controller3 = new AccountController(Session, service);
			controller3.Url = CreateUrlHelper();
			controller3.LogOn(new LogOnViewModel { Email = "e@d.com", Password = "some pwd" }, string.Empty);

			loggedOn.ShouldBe(true);
		}
	}
}
