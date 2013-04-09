﻿using System;
using Castle.Windsor;
using Moq;
using Snittlistan.Web.Areas.V1.Controllers;
using Snittlistan.Web.Areas.V1.ViewModels.Account;
using Snittlistan.Web.DomainEvents;
using Snittlistan.Web.Models;
using Snittlistan.Web.Services;
using Xunit;

namespace Snittlistan.Test
{
    public class AccountController_Logon : DbTest
    {
        private readonly IWindsorContainer oldContainer;

        public AccountController_Logon()
        {
            this.oldContainer = DomainEvent.SetContainer(new WindsorContainer());
        }

        protected override void OnDispose()
        {
            DomainEvent.SetContainer(oldContainer);
        }

        [Fact]
        public void LogonReturnsView()
        {
            // Arrange
            var controller = new AccountController(Mock.Of<IAuthenticationService>()) { DocumentSession = Session };

            // Act
            var result = controller.LogOn();

            // Assert
            result.AssertViewRendered().ForView(string.Empty);
        }

        [Fact]
        public void UnknownUserCannotLogon()
        {
            var service = Mock.Of<IAuthenticationService>();
            bool cookieSet = false;
            Mock.Get(service)
                .Setup(x => x.SetAuthCookie(It.IsAny<string>(), It.IsAny<bool>()))
                .Callback(() => cookieSet = true);

            var controller = new AccountController(service) { DocumentSession = Session };
            var result = controller.LogOn(new LogOnViewModel { Email = "unknown@d.com", Password = "some pwd" }, string.Empty);

            result.AssertViewRendered().ForView(string.Empty);
            Assert.False(cookieSet);
        }

        [Fact]
        public void ActiveUserCanLogon()
        {
            bool cookieSet = false;
            Action cookieSetAction = () => cookieSet = true;
            var controller = SetupPasswordTest(cookieSetAction);

            var result = controller.LogOn(new LogOnViewModel { Email = "e@d.com", Password = "some pwd" }, string.Empty);
            Assert.False(controller.ModelState.ContainsKey("Email"));
            Assert.False(controller.ModelState.ContainsKey("Password"));
            result.AssertActionRedirect().ToController("Home").ToAction("Index");

            Assert.True(cookieSet);
        }

        [Fact]
        public void InactiveUserCannotLogon()
        {
            Session.Store(new User(firstName: "f", lastName: "l", email: "e@d.com", password: "pwd"));
            Session.SaveChanges();

            bool loggedOn = false;
            var service = Mock.Of<IAuthenticationService>();
            Mock.Get(service).Setup(s => s.SetAuthCookie(It.IsAny<string>(), It.IsAny<bool>())).Callback(
                () => loggedOn = true);
            var controller = new AccountController(service)
                {
                    DocumentSession = Session,
                    Url = CreateUrlHelper()
                };
            var result = controller.LogOn(
                new LogOnViewModel
                    {
                        Email = "e@d.com",
                        Password = "pwd"
                    },
                string.Empty);
            Assert.True(controller.ModelState.ContainsKey("Inactive"));
            result.AssertViewRendered().ForView(string.Empty);

            Assert.False(loggedOn);
        }

        [Fact]
        public void WrongPasswordRedisplaysForm()
        {
            var cookieSet = false;
            Action cookieSetAction = () => cookieSet = true;
            var controller = SetupPasswordTest(cookieSetAction);

            var result = controller.LogOn(new LogOnViewModel { Email = "e@d.com", Password = "some other pwd" }, string.Empty);

            Assert.False(controller.ModelState.ContainsKey("Email"));
            Assert.True(controller.ModelState.ContainsKey("Password"));
            Assert.False(cookieSet);
            result.AssertViewRendered().ForView(string.Empty);
        }

        private AccountController SetupPasswordTest(Action cookieSetAction)
        {
            // create an active user
            this.CreateActivatedUser("F", "L", "e@d.com", "some pwd");

            var service = Mock.Of<IAuthenticationService>();
            Mock.Get(service).Setup(
                x => x.SetAuthCookie(It.Is<string>(s => s == "e@d.com"), It.Is<bool>(b => b == false))).Callback(
                    cookieSetAction);

            var controller = new AccountController(service)
                {
                    DocumentSession = Session,
                    Url = CreateUrlHelper()
                };
            return controller;
        }
    }
}