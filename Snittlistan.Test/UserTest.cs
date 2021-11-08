﻿#nullable enable

namespace Snittlistan.Test
{
    using NUnit.Framework;
    using Snittlistan.Queue.Messages;
    using Snittlistan.Web.Models;

    [TestFixture]
    public class UserTest
    {
        [Test]
        public void ShouldNotBeActiveWhenCreated()
        {
            User user = new("first name", "last name", "email", "password");
            Assert.False(user.IsActive);
        }

        [Test]
        public void ShouldRaiseEventWhenCreated()
        {
            NewUserCreatedTask? createdEvent = null;
            User user = new("first name", "last name", "email", "password") { Id = "users-2" };
            user.Initialize(e => createdEvent = (NewUserCreatedTask)e);

            Assert.NotNull(createdEvent);
            Assert.That(createdEvent!.Email, Is.EqualTo("email"));
            Assert.That(createdEvent.UserId, Is.EqualTo("users-2"));
        }

        [Test]
        public void CanActivate()
        {
            User user = new("F", "L", "e@d.com", "pwd");
            user.Activate();
            Assert.True(user.IsActive);
        }
    }
}
