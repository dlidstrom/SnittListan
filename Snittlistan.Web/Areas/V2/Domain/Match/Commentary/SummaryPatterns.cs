using System;
using System.Collections.Generic;
using System.Linq;

namespace Snittlistan.Web.Areas.V2.Domain.Match.Commentary
{
    public static class SummaryPatterns
    {
        public static SummaryPattern[] Create(Dictionary<string, Player> players)
        {
            Func<SeriesScores[], string> seriesFormatter = seriesScores =>
            {
                var result = $"Serierna slutade {string.Join(", ", seriesScores.Select(x => $"{x.FormattedDeltaResult} ({x.TeamPins}-{x.OpponentPins})"))}.";
                return result;
            };

            var summaryPatterns = new[]
            {
                new SummaryPattern("20-0")
                {
                    NumberOfSeries = 4,
                    MatchWon = MatchResultType.Win,
                    TeamScore = (teamScore, opponentScore, seriesScores) => teamScore == 20,
                    OpponentScore = (teamScore, opponentScore) => true,
                    Commentary = seriesScores =>
                    {
                        var sentences = new List<string>
                        {
                            $"Motst�ndarna hade inget att s�ga emot d� resultatet blev {seriesScores[3].FormattedResult}. Stark insats av laget d�r allts� alla spelare to 4 po�ng!",
                            seriesFormatter.Invoke(seriesScores)
                        };

                        return string.Join(" ", sentences);
                    }
                },
                new SummaryPattern("[14-19]-x")
                {
                    NumberOfSeries = 4,
                    MatchWon = MatchResultType.Win,
                    TeamScore = (teamScore, opponentScore, seriesScores) => teamScore >= 14 && teamScore < 20,
                    OpponentScore = (teamScore, opponentScore) => true,
                    Commentary = seriesScores =>
                    {
                        var sentences = new List<string>
                        {
                            $"Motst�ndarna blev �verk�rda med resultatet {seriesScores[3].FormattedResult}.",
                            seriesFormatter.Invoke(seriesScores)
                        };
                        return string.Join(" ", sentences);
                    }
                },
                new SummaryPattern("[9-13]-x win")
                {
                    NumberOfSeries = 4,
                    MatchWon = MatchResultType.Win,
                    TeamScore = (teamScore, opponentScore, seriesScores)
                        => teamScore < 14 && teamScore > opponentScore,
                    OpponentScore = (teamScore, opponentScore) => true,
                    Commentary = seriesScores =>
                    {
                        var lastLastLastSeries = seriesScores[seriesScores.Length-3];
                        var lastLastSeries = seriesScores[seriesScores.Length-2];
                        var lastSeries = seriesScores.Last();

                        List<string> sentences;
                        if (lastLastSeries.OpponentScoreTotal < 10
                            && lastLastSeries.TeamScoreTotal >= 10)
                        {
                            sentences = new List<string>
                            {
                                $"Matchen vanns redan efter tre serier och slutade med resultatet {seriesScores[3].FormattedResult}."
                            };
                        }
                        else
                        {
                            sentences = new List<string>
                            {
                                $"Matchen vanns med resultatet {seriesScores[3].FormattedResult}."
                            };
                        }

                        var greatSeries = seriesScores.FirstOrDefault(x => x.TeamScoreDelta == 5 && x.TeamScoreTotal <= 10);
                        if (greatSeries != null)
                        {
                            sentences.Add(
                                $"Grunden till vinsten lades i serie {greatSeries.SerieNumber} d� po�ngst�llningen var {greatSeries.FormattedResult} efter {greatSeries.FormattedDeltaResult}.");
                        }
                        else if (lastLastLastSeries.OpponentScoreTotal > 5)
                        {
                            sentences.Add(
                                $"Laget stod f�r en stark upph�mtning d� matchen v�ndes i serie 2 n�r det stod {lastLastLastSeries.FormattedResult}.");
                        }

                        if (lastLastSeries.OpponentScoreTotal < 10
                            && lastLastSeries.TeamScoreTotal <= 10
                            && lastSeries.OpponentScoreDelta > lastSeries.TeamScoreDelta)
                        {
                            var winningPlayers = lastSeries.PlayerResults.Where(x => x.Score > 0);
                            var nicknames = string.Join(
                                " och ",
                                winningPlayers.Select(x => players[x.PlayerId].Nickname));
                            var sentence = $"{nicknames} avgjorde matchen med en stark insats i sista serien.";
                            sentences.Add(sentence);
                        }

                        if (lastLastSeries.OpponentScoreTotal == 7
                            && lastLastSeries.TeamScoreTotal == 8)
                        {
                            sentences.Add("Laget avgjorde matchen med en stark avslutning i sista serien.");
                        }

                        sentences.Add(seriesFormatter.Invoke(seriesScores));
                        return string.Join(" ", sentences);
                    }
                },
                new SummaryPattern("all 4 series losses")
                {
                    NumberOfSeries = 4,
                    MatchWon = MatchResultType.Loss,
                    TeamScore = (teamScore, opponentScore, seriesScores) => true,
                    OpponentScore = (teamScore, opponentScore) => true,
                    Commentary = seriesScores =>
                    {
                        var sentences = new List<string>
                        {
                            $"Matchen slutade {seriesScores[3].FormattedResult}."
                        };

                        var lossSeries = seriesScores.Take(3)
                                                     .Skip(1)
                                                     .FirstOrDefault(x => x.TeamScoreDelta == 0
                                                                          && x.TeamScoreTotal <= 5
                                                                          && x.OpponentScoreTotal <= 10);
                        if (lossSeries != null)
                        {
                            var sentence = $"Motst�ndarna lade grunden till vinsten genom att g�ra {lossSeries.FormattedInvertedDeltaResult} i serie {lossSeries.SerieNumber} vilket resulterade i po�ngst�llningen {lossSeries.FormattedResult}.";
                            sentences.Add(sentence);
                        }

                        sentences.Add(seriesFormatter.Invoke(seriesScores));
                        return string.Join(" ", sentences);
                    }
                },
                new SummaryPattern("all 4 series draws")
                {
                    NumberOfSeries = 4,
                    MatchWon = MatchResultType.Draw,
                    TeamScore = (teamScore, opponentScore, seriesScores) => true,
                    OpponentScore = (teamScore, opponentScore) => true,
                    Commentary = seriesScores =>
                    {
                        var sentences = new List<string>();
                        var lastSeries = seriesScores.Last();
                        var lastLastSeries = seriesScores[seriesScores.Length-2];
                        var lastLastLastSeries = seriesScores[seriesScores.Length-3];
                        if (lastLastLastSeries.OpponentScoreTotal >= 7)
                        {
                            sentences.Add(
                                $"Laget stod f�r en fin upph�mtning d� det stod {lastLastLastSeries.FormattedResult} efter halva matchen.");
                        }

                        if (lastLastSeries.OpponentScoreTotal < lastLastSeries.TeamScoreTotal)
                        {
                            if (lastLastSeries.OpponentScoreDelta >= 4)
                            {
                                sentences.Add(
                                    $"Det stod {lastLastSeries.FormattedResult} efter 3 serier men motst�ndarna ryckte i sista serien och laget fick n�ja sig med oavgjort.");
                            }
                            else
                            {
                                sentences.Add("Matchen slutade oavgjort efter att motst�ndarna vann sista serien.");
                            }
                        }
                        else if (lastLastSeries.OpponentScoreTotal > lastLastSeries.TeamScoreTotal)
                        {
                            if (lastLastSeries.OpponentScoreTotal == 10)
                            {
                                sentences.Add("Det stod 5-10 efter 3 serier. Men i sista serien k�rdes motst�ndarna �ver f�r ett oavgjort resultat!");
                            }
                            else
                            {
                                sentences.Add("Matchen slutade oavgjort efter vinst i sista serien.");
                            }
                        }
                        else
                        {
                            sentences.Add(
                                $"Matchen slutade oavgjort med ovanliga resultatet {lastSeries.FormattedResult}.");
                        }

                        sentences.Add(seriesFormatter.Invoke(seriesScores));
                        return string.Join(" ", sentences);
                    }
                },
                new SummaryPattern("all 3 series wins")
                {
                    NumberOfSeries = 3,
                    MatchWon = MatchResultType.Win,
                    TeamScore = (teamScore, opponentScore, seriesScores) => true,
                    OpponentScore = (teamScore, opponentScore) => true,
                    Commentary = seriesScores => $"Matchen vanns redan efter 3 serier med resultatet {seriesScores[2].FormattedResult}."
                },
                new SummaryPattern("all 1 series wins")
                {
                    NumberOfSeries = 1,
                    MatchWon = MatchResultType.Win,
                    TeamScore = (teamScore, opponentScore, seriesScores) => true,
                    OpponentScore = (teamScore, opponentScore) => true,
                    Commentary = seriesScores => ""
                }
            };

            return summaryPatterns;
        }
    }
}