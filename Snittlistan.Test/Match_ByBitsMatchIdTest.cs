﻿namespace Snittlistan.Test
{
    using Snittlistan.Web.Helpers;

    using Xunit;

    public class Match_ByBitsMatchIdTest : DbTest
    {
        [Fact]
        public void ShouldFindBitsId()
        {
            // Arrange
            var match = DbSeed.Create8x4Match();
            Session.Store(match);
            Session.SaveChanges();

            // Act
            bool found = Session.BitsIdExists(3003231);

            // Assert
            Assert.True(found);
        }
    }
}
