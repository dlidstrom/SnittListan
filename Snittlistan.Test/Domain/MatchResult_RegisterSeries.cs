﻿using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Snittlistan.Web.Areas.V2.Domain;
using Snittlistan.Web.Areas.V2.Domain.Match;
using Snittlistan.Web.Areas.V2.Domain.Match.Events;
using Snittlistan.Web.Areas.V2.Indexes;
using Snittlistan.Web.Areas.V2.ReadModels;
using Snittlistan.Web.DomainEvents;

namespace Snittlistan.Test.Domain
{
    [TestFixture]
    public class MatchResult_RegisterSeries
    {
        private Roster roster;
        private MatchResult matchResult;
        private MatchRegisteredEvent ev;

        [SetUp]
        public void SetUp()
        {
            // Arrange
            var players = new[]
            {
                new Player("n", "e@d.com", Player.Status.Active, -1, null)
                {
                    Id = "p1"
                },
                new Player("n", "e@d.com", Player.Status.Active, -1, null)
                {
                    Id = "p2"
                },
                new Player("n", "e@d.com", Player.Status.Active, -1, null)
                {
                    Id = "p3"
                },
                new Player("n", "e@d.com", Player.Status.Active, -1, null)
                {
                    Id = "p4"
                },
                new Player("n", "e@d.com", Player.Status.Active, -1, null)
                {
                    Id = "p5"
                },
                new Player("n", "e@d.com", Player.Status.Active, -1, null)
                {
                    Id = "p6"
                },
                new Player("n", "e@d.com", Player.Status.Active, -1, null)
                {
                    Id = "p7"
                },
                new Player("n", "e@d.com", Player.Status.Active, -1, null)
                {
                    Id = "p8"
                },
            };
            roster = new Roster(2012, 11, 1, "H", "A", "L", "A", new DateTime(2012, 2, 3), false)
            {
                Id = "rosters-1",
                Players = players.Select(x => x.Id).ToList()
            };
            matchResult = new MatchResult(roster, 9, 11, 123);

            // Act
            var series = new[]
            {
                new MatchSerie(
                    1,
                    new List<MatchTable>
                    {
                        new MatchTable(1, new MatchGame("p1", 169, 0, 0), new MatchGame("p2", 0, 0, 0), 0),
                        new MatchTable(2, new MatchGame("p3", 0, 0, 0), new MatchGame("p4", 170, 0, 0), 0),
                        new MatchTable(3, new MatchGame("p5", 0, 0, 0), new MatchGame("p6", 0, 0, 0), 0),
                        new MatchTable(4, new MatchGame("p7", 200, 0, 0), new MatchGame("p8", 0, 0, 0), 0),
                    }),
                new MatchSerie(
                    2,
                    new List<MatchTable>
                    {
                        new MatchTable(1, new MatchGame("p1", 169, 0, 0), new MatchGame("p2", 0, 0, 0), 0),
                        new MatchTable(2, new MatchGame("p3", 0, 0, 0), new MatchGame("p4", 170, 0, 0), 0),
                        new MatchTable(3, new MatchGame("p5", 0, 0, 0), new MatchGame("p6", 0, 0, 0), 0),
                        new MatchTable(4, new MatchGame("p7", 200, 0, 0), new MatchGame("p8", 0, 0, 0), 0),
                    })
            };

            using (DomainEvent.TestWith(e => ev = (MatchRegisteredEvent)e))
            {
                var opponentSeries = new[]
                {
                    new ResultSeriesReadModel.Serie
                    {
                        Tables = new List<ResultSeriesReadModel.Table>
                        {
                            new ResultSeriesReadModel.Table
                            {
                                Game1 = new ResultSeriesReadModel.Game(),
                                Game2 = new ResultSeriesReadModel.Game()
                            },
                            new ResultSeriesReadModel.Table
                            {
                                Game1 = new ResultSeriesReadModel.Game(),
                                Game2 = new ResultSeriesReadModel.Game()
                            },
                            new ResultSeriesReadModel.Table
                            {
                                Game1 = new ResultSeriesReadModel.Game(),
                                Game2 = new ResultSeriesReadModel.Game()
                            },
                            new ResultSeriesReadModel.Table
                            {
                                Game1 = new ResultSeriesReadModel.Game(),
                                Game2 = new ResultSeriesReadModel.Game()
                            }
                        }
                    },
                    new ResultSeriesReadModel.Serie
                    {
                        Tables = new List<ResultSeriesReadModel.Table>
                        {
                            new ResultSeriesReadModel.Table
                            {
                                Game1 = new ResultSeriesReadModel.Game(),
                                Game2 = new ResultSeriesReadModel.Game()
                            },
                            new ResultSeriesReadModel.Table
                            {
                                Game1 = new ResultSeriesReadModel.Game(),
                                Game2 = new ResultSeriesReadModel.Game()
                            },
                            new ResultSeriesReadModel.Table
                            {
                                Game1 = new ResultSeriesReadModel.Game(),
                                Game2 = new ResultSeriesReadModel.Game()
                            },
                            new ResultSeriesReadModel.Table
                            {
                                Game1 = new ResultSeriesReadModel.Game(),
                                Game2 = new ResultSeriesReadModel.Game()
                            }
                        }
                    }
                };
                matchResult.RegisterSeries(
                    series,
                    opponentSeries,
                    players,
                    new Dictionary<string, ResultForPlayerIndex.Result>());
            }
        }

        [Test]
        public void CanRegisterAllSeries()
        {
            // Assert
            var changes = matchResult.GetUncommittedChanges();
            Assert.That(changes, Has.Length.EqualTo(4));
            Assert.IsAssignableFrom<SerieRegistered>(changes[1]);
        }

        [Test]
        public void RaisesDomainEvent()
        {
            // Assert
            Assert.That(ev, Is.Not.Null);
        }
    }
}