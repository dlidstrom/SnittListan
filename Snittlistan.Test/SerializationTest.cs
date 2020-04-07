﻿using System.IO;
using System.Text;
using NUnit.Framework;
using Raven.Imports.Newtonsoft.Json;
using Snittlistan.Web.Areas.V1.Models;
using Snittlistan.Web.Models;

namespace Snittlistan.Test
{
    [TestFixture]
    public class SerializationTest : DbTest
    {
        [Test]
        public void CanSerialize8X4Match()
        {
            // Arrange
            var serializer = Store.Conventions.CreateSerializer();
            var builder = new StringBuilder();

            // Act
            serializer.Serialize(new StringWriter(builder), DbSeed.Create8x4Match());
            string text = builder.ToString();
            var match = serializer.Deserialize<Match8x4>(new JsonTextReader(new StringReader(text)));

            // Assert
            TestData.VerifyTeam(match.AwayTeam);
        }

        [Test]
        public void CanSerialize4X4Match()
        {
            // Arrange
            var serializer = Store.Conventions.CreateSerializer();
            var builder = new StringBuilder();

            // Act
            serializer.Serialize(new StringWriter(builder), DbSeed.Create4x4Match());
            string text = builder.ToString();
            var match = serializer.Deserialize<Match4x4>(new JsonTextReader(new StringReader(text)));

            // Assert
            TestData.VerifyTeam(match.HomeTeam);
        }

        [Test]
        public void CanSerializeUser()
        {
            // Arrange
            var user = new User("firstName", "lastName", "e@d.com", "some-pass");

            // Act
            using (var session = Store.OpenSession())
            {
                session.Store(user);
                session.SaveChanges();
            }

            // Assert
            using (var session = Store.OpenSession())
            {
                var loadedUser = session.Load<User>(user.Id);
                Assert.True(loadedUser.ValidatePassword("some-pass"), "Password validation failed");
            }
        }
    }
}