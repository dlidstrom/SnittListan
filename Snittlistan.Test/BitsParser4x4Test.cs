﻿using System;
using NUnit.Framework;
using Snittlistan.Test.Properties;
using Snittlistan.Web.Areas.V2.Domain;
using Snittlistan.Web.Areas.V2.ReadModels;

namespace Snittlistan.Test
{
    [TestFixture]
    public class BitsParser4X4Test
    {
        private readonly Player[] homePlayers;
        private readonly Player[] awayPlayers;

        public BitsParser4X4Test()
        {
            homePlayers = new[]
            {
                new Player("Markus Norbeck", "e@d.com", Player.Status.Active, 0, null) { Id = "player-1" },
                new Player("Lars Norbeck", "e@d.com", Player.Status.Active, 0, null) { Id = "player-2" },
                new Player("Daniel Solvander", "e@d.com", Player.Status.Active, 0, null) { Id = "player-3" },
                new Player("Matz Classon", "e@d.com", Player.Status.Active, 0, null) { Id = "player-4" }
            };
            awayPlayers = new[]
            {
                new Player("Tobias Wiklund", "e@d.com", Player.Status.Active, 0, null) { Id = "player-5" },
                new Player("Wiktor Svensson", "e@d.com", Player.Status.Active, 0, null) { Id = "player-6" },
                new Player("Moa Nilsson", "e@d.com", Player.Status.Active, 0, null) { Id = "player-7" },
                new Player("Rebecka Wahlström", "e@d.com", Player.Status.Active, 0, null) { Id = "player-8" },
                new Player("Hanna Engevall", "e@d.com", Player.Status.Active, 0, null) { Id = "player-9" }
            };
        }

        [Test]
        public void CanParseHomeTeam()
        {
            // Arrange
            const string Team = "Fredrikshof B";

            // Act
            var result = new BitsParser(homePlayers).Parse4(Resources.Id3060835, Team);
            Assert.That(result.TeamScore, Is.EqualTo(18));
            Assert.That(result.OpponentScore, Is.EqualTo(1));
            var series = result.Series;

            // Assert
            Assert.That(series.Length, Is.EqualTo(4));
            var serie1 = series[0];
            Assert.That(serie1.Games, Has.Count.EqualTo(4));
            VerifyGame(serie1.Games[0], Tuple.Create(0, "player-1", 138));
            VerifyGame(serie1.Games[1], Tuple.Create(1, "player-2", 178));
            VerifyGame(serie1.Games[2], Tuple.Create(1, "player-3", 183));
            VerifyGame(serie1.Games[3], Tuple.Create(1, "player-4", 131));

            var serie2 = series[1];
            Assert.That(serie2.Games, Has.Count.EqualTo(4));
            VerifyGame(serie2.Games[0], Tuple.Create(1, "player-3", 152));
            VerifyGame(serie2.Games[1], Tuple.Create(1, "player-4", 189));
            VerifyGame(serie2.Games[2], Tuple.Create(1, "player-1", 205));
            VerifyGame(serie2.Games[3], Tuple.Create(1, "player-2", 136));

            var serie3 = series[2];
            Assert.That(serie3.Games, Has.Count.EqualTo(4));
            VerifyGame(serie3.Games[0], Tuple.Create(1, "player-4", 223));
            VerifyGame(serie3.Games[1], Tuple.Create(1, "player-3", 251));
            VerifyGame(serie3.Games[2], Tuple.Create(1, "player-2", 158));
            VerifyGame(serie3.Games[3], Tuple.Create(1, "player-1", 149));

            var serie4 = series[3];
            Assert.That(serie4.Games, Has.Count.EqualTo(4));
            VerifyGame(serie4.Games[0], Tuple.Create(1, "player-2", 179));
            VerifyGame(serie4.Games[1], Tuple.Create(1, "player-1", 181));
            VerifyGame(serie4.Games[2], Tuple.Create(1, "player-4", 183));
            VerifyGame(serie4.Games[3], Tuple.Create(0, "player-3", 167));
        }

        [Test]
        public void CanParseAwayTeam()
        {
            // Arrange
            const string Team = "Trippel XXX";

            // Act
            var result = new BitsParser(awayPlayers).Parse4(Resources.Id3060835, Team);
            Assert.That(result.TeamScore, Is.EqualTo(1));
            Assert.That(result.OpponentScore, Is.EqualTo(18));
            var series = result.Series;

            // Assert
            Assert.That(series.Length, Is.EqualTo(4));
            var serie1 = series[0];
            Assert.That(serie1.Games.Count, Is.EqualTo(4));
            VerifyGame(serie1.Games[0], Tuple.Create(1, "player-5", 160));
            VerifyGame(serie1.Games[1], Tuple.Create(0, "player-6", 117));
            VerifyGame(serie1.Games[2], Tuple.Create(0, "player-7", 139));
            VerifyGame(serie1.Games[3], Tuple.Create(0, "player-8", 83));

            var serie2 = series[1];
            Assert.That(serie2.Games.Count, Is.EqualTo(4));
            VerifyGame(serie2.Games[0], Tuple.Create(0, "player-8", 122));
            VerifyGame(serie2.Games[1], Tuple.Create(0, "player-7", 156));
            VerifyGame(serie2.Games[2], Tuple.Create(0, "player-6", 101));
            VerifyGame(serie2.Games[3], Tuple.Create(0, "player-5", 133));

            var serie3 = series[2];
            Assert.That(serie3.Games.Count, Is.EqualTo(4));
            VerifyGame(serie3.Games[0], Tuple.Create(0, "player-6", 148));
            VerifyGame(serie3.Games[1], Tuple.Create(0, "player-5", 142));
            VerifyGame(serie3.Games[2], Tuple.Create(0, "player-9", 118));
            VerifyGame(serie3.Games[3], Tuple.Create(0, "player-7", 136));

            var serie4 = series[3];
            Assert.That(serie4.Games.Count, Is.EqualTo(4));
            VerifyGame(serie4.Games[0], Tuple.Create(0, "player-7", 134));
            VerifyGame(serie4.Games[1], Tuple.Create(0, "player-9", 119));
            VerifyGame(serie4.Games[2], Tuple.Create(0, "player-5", 122));
            VerifyGame(serie4.Games[3], Tuple.Create(0, "player-6", 167));
        }

        [Test]
        public void DoesNotParseUnfinishedGame()
        {
            // Arrange
            const string Team = "Värtans IK B";

            // Act
            var result = new BitsParser(awayPlayers).Parse4(Resources.Id3148481, Team);

            // Assert
            Assert.That(result, Is.Null);
        }

        private static void VerifyGame(ResultSeries4ReadModel.Game game, Tuple<int, string, int> expected)
        {
            Assert.That(expected.Item1, Is.EqualTo(game.Score));
            Assert.That(expected.Item2, Is.EqualTo(game.Player));
            Assert.That(expected.Item3, Is.EqualTo(game.Pins));
        }
    }
}