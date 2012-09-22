﻿namespace Snittlistan.Test
{
    using Moq;

    using Snittlistan.Web.Events;
    using Snittlistan.Web.Handlers;
    using Snittlistan.Web.Models;
    using Snittlistan.Web.Services;

    using Xunit;

    public class SendRegistrationEmailHandlerTest
    {
        [Fact(Skip = "Cannot initialize ViewEngines")]
        public void ShouldSendMail()
        {
            // Arrange
            var service = Mock.Of<IEmailService>();
            var recipient = It.Is<string>(s => s == "e@d.com");
            var subject = It.Is<string>(s => s == "subject");
            bool mailSent = false;
            Mock.Get(service)
                .Setup(s => s.SendMail(recipient, subject, It.IsAny<string>()))
                .Callback(() => mailSent = true);

            // Act
            var handler = new SendRegistrationEmailHandler(service);
            handler.Handle(new NewUserCreatedEvent
            {
                User = new User("F", "L", "e@d.com", "some pwd")
            });

            // Assert
            Assert.True(mailSent);
        }
    }
}