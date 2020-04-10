using System;
using System.Collections.Generic;
using System.Linq;
using EventStoreLite;
using Moq;
using NUnit.Framework;
using Snittlistan.Test.ApiControllers;
using Snittlistan.Web.Areas.V2.Commands;
using Snittlistan.Web.Areas.V2.Domain;
using Snittlistan.Web.Areas.V2.Domain.Match;
using Snittlistan.Web.Areas.V2.Domain.Match.Events;
using Snittlistan.Web.Areas.V2.ReadModels;

namespace Snittlistan.Test.Domain
{
    using System.Threading.Tasks;

    [TestFixture]
    public class MatchResult_MatchCommentary : WebApiIntegrationTest
    {
        private static readonly Player[] Players =
        {
            new Player("Alf Kindblom", "e@d.com", Player.Status.Active, -1, "Affe", new string[0]),
            new Player("Bengt Solvander", "e@d.com", Player.Status.Active, -1, "Sollan", new string[0]),
            new Player("Christer Holmstr�m", "e@d.com", Player.Status.Active, -1, "Holmis", new string[0]),
            new Player("Christer Liedholm", "e@d.com", Player.Status.Active, -1, "Chrille", new string[0]),
            new Player("Claes Trank�rr", "e@d.com", Player.Status.Active, -1, "Tranan", new string[0]),
            new Player("Daniel Lidstr�m", "e@d.com", Player.Status.Active, -1, "Lidas", new string[0]),
            new Player("Daniel Solvander", "e@d.com", Player.Status.Active, -1, "Solen", new string[0]),
            new Player("Hans Norbeck", "e@d.com", Player.Status.Active, -1, "Hasse", new string[0]),
            new Player("H�kan Gustavsson", "e@d.com", Player.Status.Active, -1, "Hockey", new string[0]),
            new Player("Karl-Erik Frick", "e@d.com", Player.Status.Active, -1, "Kalle", new string[0]),
            new Player("Kjell Jansson", "e@d.com", Player.Status.Active, -1, "Jansson", new string[0]),
            new Player("Kjell Johansson", "e@d.com", Player.Status.Active, -1, "Hammarn", new string[0]),
            new Player("Kjell Persson", "e@d.com", Player.Status.Active, -1, "KP", new string[0]),
            new Player("Lars H�glin", "e@d.com", Player.Status.Active, -1, "H�ken", new string[0]),
            new Player("Lars Magnusson", "e@d.com", Player.Status.Active, -1, "Lasse Magnus", new string[0]),
            new Player("Lars Norbeck", "e@d.com", Player.Status.Active, -1, "Norpan", new string[0]),
            new Player("Lars �berg", "e@d.com", Player.Status.Active, -1, "�berg", new string[0]),
            new Player("Lennart Axelsson", "e@d.com", Player.Status.Active, -1, "Laxen", new string[0]),
            new Player("Magnus Sj�holm", "e@d.com", Player.Status.Active, -1, "Masken", new string[0]),
            new Player("Markus Norbeck", "e@d.com", Player.Status.Active, -1, "Markus", new string[0]),
            new Player("Mathias Ernest", "e@d.com", Player.Status.Active, -1, "Ernest", new string[0]),
            new Player("Matz Classon", "e@d.com", Player.Status.Active, -1, "Classon", new string[0]),
            new Player("Mikael Axelsson", "e@d.com", Player.Status.Active, -1, "Micke", new string[0]),
            new Player("Per-Erik Freij", "e@d.com", Player.Status.Active, -1, "Perre", new string[0]),
            new Player("Peter Engborg", "e@d.com", Player.Status.Active, -1, "Peter E", new string[0]),
            new Player("Peter Sj�berg", "e@d.com", Player.Status.Active, -1, "Peter S", new string[0]),
            new Player("Ralph Svensson", "e@d.com", Player.Status.Active, -1, "Raffe", new string[0]),
            new Player("Stefan Markenfelt", "e@d.com", Player.Status.Active, -1, "Marken", new string[0]),
            new Player("Stefan Traav", "e@d.com", Player.Status.Active, -1, "Traav", new string[0]),
            new Player("Thomas Wallgren", "e@d.com", Player.Status.Active, -1, "TW", new string[0]),
            new Player("Thomas Gurell", "e@d.com", Player.Status.Active, -1, "Gurkan", new string[0]),
            new Player("Tomas Vikbro", "e@d.com", Player.Status.Active, -1, "Gusten", new string[0]),
            new Player("Tony Nordstr�m", "e@d.com", Player.Status.Active, -1, "Tony", new string[0]),
            new Player("Torbj�rn Jensen", "e@d.com", Player.Status.Active, -1, "Tobbe", new string[0])
        };

        [TestCaseSource(nameof(BitsMatchIdAndCommentaries))]
        public async Task CorrectTurn(TestCase testCase)
        {
            // Act
            var bitsParser = new BitsParser(Players);
            var content = await BitsGateway.GetMatch(testCase.BitsMatchId);
            var parseResult = bitsParser.Parse(content, "Fredrikshof");

            // Assert
            Assert.That(parseResult.Turn, Is.EqualTo(testCase.Turn));
        }

        [TestCaseSource(nameof(BitsMatchIdAndCommentaries))]
        public async Task MatchCommentarySummaryText(TestCase testCase)
        {
            // Act
            var matchResult = await  Act(testCase);

            // Assert
            var changes = matchResult.GetUncommittedChanges();
            var matchCommentaryEvent = (MatchCommentaryEvent)changes.Single(x => x is MatchCommentaryEvent);
            Assert.That(matchCommentaryEvent.SummaryText, Is.EqualTo(testCase.ExpectedSummaryText));
        }

        [TestCaseSource(nameof(BitsMatchIdAndCommentaries))]
        public async Task MatchCommentaryBodyText(TestCase testCase)
        {
            // Act
            var matchResult = await Act(testCase);

            // Assert
            var changes = matchResult.GetUncommittedChanges();
            var matchCommentaryEvent = (MatchCommentaryEvent)changes.Single(x => x is MatchCommentaryEvent);
            Assert.That(string.Join(" ", matchCommentaryEvent.BodyText), Is.EqualTo(testCase.ExpectedBodyText));
        }

        private async Task<MatchResult> Act(TestCase testCase)
        {
            // Arrange
            Transact(session =>
            {
                foreach (var player in Players)
                {
                    session.Store(player);
                }
            });
            var bitsParser = new BitsParser(Players);

            var content = await BitsGateway.GetMatch(testCase.BitsMatchId);
            var parseResult = bitsParser.Parse(content, "Fredrikshof");
            var rosterPlayerIds = new HashSet<string>(
                parseResult.Series.SelectMany(x => x.Tables.SelectMany(y => new[] { y.Game1.Player, y.Game2.Player })));
            var roster = new Roster(2017, 1, testCase.BitsMatchId, "Fredrikshof", "A", string.Empty, string.Empty, DateTime.Now, false, OilPatternInformation.Empty)
            {
                Id = "rosters-1",
                Players = rosterPlayerIds.ToList()
            };
            var command = new RegisterMatchCommand(roster, parseResult);

            // prepare some results
            Transact(session =>
            {
                var nicknameToId = Players.ToDictionary(x => x.Nickname);
                var playerResults = new Dictionary<string, int[]>
                {
                    [nicknameToId["Ernest"].Id] = new[] { 205 },
                    [nicknameToId["Lidas"].Id] = new[] { 190 },
                    [nicknameToId["Laxen"].Id] = new[] { 190 },
                    [nicknameToId["Lasse Magnus"].Id] = new[] { 205 },
                    [nicknameToId["Norpan"].Id] = new[] { 190 },
                    [nicknameToId["Traav"].Id] = new[] { 190 }
                };
                foreach (var playerId in playerResults.Keys)
                {
                    for (var bitsMatchId = 10; bitsMatchId < 15; bitsMatchId++)
                    {
                        var resultForPlayer = new ResultForPlayerReadModel(2017, playerId, bitsMatchId, null, DateTime.Now);
                        foreach (var playerResult in playerResults[playerId])
                        {
                            resultForPlayer.AddGame(1, new MatchGame(playerId, playerResult, 0, 0));
                            resultForPlayer.AddGame(1, new MatchGame(playerId, playerResult, 0, 0));
                            resultForPlayer.AddGame(1, new MatchGame(playerId, playerResult, 0, 0));
                            resultForPlayer.AddGame(1, new MatchGame(playerId, playerResult, 0, 0));
                        }

                        session.Store(resultForPlayer);
                    }
                }
            });

            // Act
            MatchResult matchResult = null;
            Transact(session =>
            {
                var eventStoreSession = Mock.Of<IEventStoreSession>();
                Mock.Get(eventStoreSession)
                    .Setup(x => x.Store(It.IsAny<AggregateRoot>()))
                    .Callback((AggregateRoot ar) => matchResult = (MatchResult)ar);
                command.Execute(session, eventStoreSession, o => { });
            });

            return matchResult;
        }

        private static IEnumerable<TestCase> BitsMatchIdAndCommentaries
        {
            get
            {
                yield return new TestCase(3050553, 1, "10-10", "Laget stod f�r en fin upph�mtning d� det stod 3-7 efter halva matchen. Det stod 5-10 efter 3 serier. Men i sista serien k�rdes motst�ndarna �ver f�r ett oavgjort resultat! Serierna slutade 2-3 (1430-1620), 1-4 (1528-1693), 2-3 (1524-1598), 5-0 (1690-1529).", "Micke 837, Ernest 805, Tobbe 801, Peter S 772, �berg 747, Affe 740, Hasse 575 (3), Chrille 549 (3), Lasse Magnus 346 (2).");
                yield return new TestCase(3067035, 24, "10-10", "Matchen vanns redan efter 3 serier med resultatet 10-5.", "Serie 3 vanns med enbart 15 pinnar. Lidas (277), Traav (234) och Laxen (222) l�g bakom serievinsten med sina fina resultat. Traav 692 (3), Laxen 676 (3), Lidas 637 (3), Hockey 537 (3), Hammarn 524 (3), Gurkan 516 (3), Sollan 506 (3), Solen 504 (3).");
                yield return new TestCase(3122544, 15, "10-10", "Laget stod f�r en fin upph�mtning d� det stod 3-7 efter halva matchen. Matchen slutade oavgjort efter vinst i sista serien. Serierna slutade 2-3 (1461-1514), 1-4 (1403-1541), 4-1 (1540-1471), 3-2 (1598-1508).", "Tranan och Hasse blev matchens viktigaste spelare med 4 vunna serier vardera. Tranan 815, Jansson 803, Hasse 762, Traav 762, Holmis 747, �berg 740, Gusten 738, Sollan 514 (3), Laxen 121 (1).");
                yield return new TestCase(3128352, 5, "10-10", "Matchen slutade oavgjort efter vinst i sista serien. Serierna slutade 4-1 (1365-1236), 1-4 (1181-1251), 2-3 (1335-1349), 3-2 (1296-1258).", "Jansson 741, Classon 710, Norpan 665, Markus 634, Perre 632, Tony 607, Peter E 598, Marken 590.");
                yield return new TestCase(3128387, 12, "10-10", "Det stod 9-6 efter 3 serier men motst�ndarna ryckte i sista serien och laget fick n�ja sig med oavgjort. Serierna slutade 4-1 (1470-1397), 4-1 (1343-1296), 1-4 (1398-1502), 1-4 (1329-1441).", "Sollan 791, Gusten 736, Norpan 679, Kalle 674, Perre 651, Hammarn 525 (3), Raffe 520 (3), Classon 501 (3), Peter E 463 (3).");
                yield return new TestCase(3119116, 4, "20-0", "Motst�ndarna hade inget att s�ga emot d� resultatet blev 20-0. Stark insats av laget d�r allts� alla spelare to 4 po�ng! Serierna slutade 5-0 (1606-1350), 5-0 (1720-1509), 5-0 (1701-1514), 5-0 (1777-1402).", "Masken, Ernest, Micke, Tobbe, Chrille, TW och Lasse Magnus blev matchens viktigaste spelare med 4 vunna serier vardera. Masken 965, Ernest 930, Micke 876, Tobbe 867, Chrille 838, TW 830, Lasse Magnus 742, Holmis 420 (2), Gurkan 336 (2).");
                yield return new TestCase(3119211, 13, "19-1", "Motst�ndarna blev �verk�rda med resultatet 19-1. Serierna slutade 5-0 (1749-1634), 5-0 (1691-1471), 4-1 (1756-1511), 5-0 (1791-1656).", "I serie 4 sm�llde Masken till med 300! Masken, Tobbe, Chrille, TW, Micke och Lasse Magnus blev matchens viktigaste spelare med 4 vunna serier vardera. Masken 980, Tobbe 954, Chrille 900, TW 889, Micke 884, Ernest 822, Lasse Magnus 780, Gurkan 426 (2), �berg 352 (2).");
                yield return new TestCase(3083803, 17, "13-7", "Matchen vanns med resultatet 13-7. Grunden till vinsten lades i serie 2 d� po�ngst�llningen var 7-3 efter 5-0. Serierna slutade 2-3 (1467-1444), 5-0 (1707-1462), 2-3 (1591-1596), 4-1 (1610-1493).", "TW 861, Traav 848, Holmis 836, Gurkan 823, Kalle 779, �berg 764, Norpan 732, Hammarn 732.");
                yield return new TestCase(3105692, 21, "16-4", "Motst�ndarna blev �verk�rda med resultatet 16-4. Serierna slutade 4-1 (1666-1512), 4-1 (1556-1501), 4-1 (1572-1446), 4-1 (1563-1465).", "I serie 1 stod Lidas (276) f�r utm�rkt insats. Lidas och Hasse blev matchens viktigaste spelare med 4 vunna serier vardera. Grattis till Lidas som uppn�dde magiska 1000 po�ng! Lidas 1003, Tranan 864, Laxen 816, Traav 795, Hasse 770, Jansson 736, Norpan 696, Hockey 518 (3), Affe 159 (1).");
                yield return new TestCase(3119140, 6, "17-3", "Motst�ndarna blev �verk�rda med resultatet 17-3. Serierna slutade 5-0 (1719-1553), 3-2 (1597-1579), 4-1 (1733-1545), 5-0 (1702-1511).", "Serie 2 vanns med enbart 18 pinnar. Lasse Magnus (238) och Ernest (222) l�g bakom serievinsten med sina fina resultat. I serie 3 stod Ernest (279) och Masken (277) f�r utm�rkta insatser. Masken, Tobbe, TW och Lasse Magnus blev matchens viktigaste spelare med 4 vunna serier vardera. Masken 944, Ernest 936, Tobbe 863, Chrille 836, TW 827, Micke 789, Lasse Magnus 787, �berg 769.");
                yield return new TestCase(3119150, 7, "6-13", "Matchen slutade 6-13. Motst�ndarna lade grunden till vinsten genom att g�ra 5-0 i serie 3 vilket resulterade i po�ngst�llningen 5-10. Serierna slutade 3-2 (1434-1406), 2-3 (1463-1501), 0-5 (1417-1560), 1-3 (1468-1474).", "Masken 760, �berg 747, Micke 743, Ernest 729, Gurkan 718, TW 708, Tobbe 700, Gusten 359 (2), Lasse Magnus 318 (2).");
                yield return new TestCase(3119219, 14, "11-8", "Matchen vanns med resultatet 11-8. Laget stod f�r en stark upph�mtning d� matchen v�ndes i serie 2 n�r det stod 4-6. Serierna slutade 1-4 (1640-1706), 3-2 (1643-1589), 4-0 (1669-1513), 3-2 (1683-1639).", "Masken 917, Ernest 890, Tobbe 868, Peter S 839, Lasse Magnus 819, �berg 784, TW 764, Gurkan 754.");
                yield return new TestCase(3152177, 2, "0-20", "Matchen slutade 0-20. Motst�ndarna lade grunden till vinsten genom att g�ra 5-0 i serie 2 vilket resulterade i po�ngst�llningen 0-10. Serierna slutade 0-5 (1662-1804), 0-5 (1529-1772), 0-5 (1604-1797), 0-5 (1544-1738).", "Ernest 890, Lasse Magnus 832, TW 822, Tobbe 805, Chrille 797, Gurkan 757, H�ken 723, Micke 713.");
                yield return new TestCase(3139850, 2, "11-9", "Matchen vanns med resultatet 11-9. Hasse och Peter S och Markus och Laxen avgjorde matchen med en stark insats i sista serien. Serierna slutade 4-1 (1519-1370), 2-3 (1446-1555), 3-2 (1551-1525), 2-3 (1485-1564).", "Holmis 855, Masken 818, Lidas 804, Peter S 774, Laxen 762, Classon 712, Hasse 645, Markus 347 (2), Gusten 284 (2).");
                yield return new TestCase(3138423, 2, "12-8", "Matchen vanns med resultatet 12-8. Laget avgjorde matchen med en stark avslutning i sista serien. Serierna slutade 4-1 (1265-1237), 1-4 (1264-1386), 3-2 (1319-1270), 4-1 (1370-1280).", "Affe 740, Kalle 714, Hammarn 660, Jansson 659, Solen 651, Perre 638, Raffe 454 (3), Tony 448 (3), Marken 254 (2).");
                yield return new TestCase(3152187, 3, "4-16", "Matchen slutade 4-16. Serierna slutade 1-4 (1318-1415), 2-3 (1516-1594), 1-4 (1395-1522), 0-5 (1367-1581).", "H�ken 789, Tobbe 731, Peter S 726, Ernest 724, TW 711, Holmis 697, Lidas 605, Laxen 474 (3), Raffe 139 (1).");
                yield return new TestCase(3139904, 3, "13-7", "Matchen vanns redan efter tre serier och slutade med resultatet 13-7. Serierna slutade 4-1 (1534-1381), 2-3 (1393-1417), 5-0 (1462-1326), 2-3 (1321-1395).", "�berg 782, KP 774, Tranan 760, Affe 744, Hockey 701, Gurkan 681, Classon 675, Hammarn 485 (3), Markus 108 (1).");
                yield return new TestCase(3139862, 7, "6-14", "Matchen slutade 6-14. Serierna slutade 1-4 (1293-1491), 1-4 (1424-1469), 2-3 (1383-1469), 2-3 (1369-1387).", "�berg 756, KP 713, Classon 667, Hasse 666, Hockey 664, Affe 663, Tranan 525 (3), Solen 494 (3), Laxen 321 (2).");
                yield return new TestCase(3152213, 6, "12-8", "Matchen vanns redan efter tre serier och slutade med resultatet 12-8. Serierna slutade 4-1 (1510-1402), 3-2 (1570-1511), 4-1 (1545-1451), 1-4 (1506-1558).", "Chrille och Ernest blev matchens viktigaste spelare med 4 vunna serier vardera. Chrille 845, Peter S 830, Tobbe 807, Ernest 785, Holmis 713, TW 709, H�ken 695, Micke 609 (3), Lasse Magnus 138 (1).");
            }
        }

        public class TestCase
        {
            public TestCase(
                int bitsMatchId,
                int turn,
                string result,
                string expectedSummaryText,
                string expectedBodyText)
            {
                BitsMatchId = bitsMatchId;
                Turn = turn;
                Result = result;
                ExpectedSummaryText = expectedSummaryText;
                ExpectedBodyText = expectedBodyText;
            }

            public int BitsMatchId { get; }

            public int Turn { get; }

            public string Result { get; }

            public string ExpectedSummaryText { get; }

            public string ExpectedBodyText { get; }

            public override string ToString()
            {
                return $"{BitsMatchId}, {Result}";
            }
        }
    }
}