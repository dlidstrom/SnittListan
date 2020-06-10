﻿namespace Snittlistan.Web.Areas.V2.Domain.Match.Commentary
{
    using System;
    using System.Linq;

    public class SummaryPattern
    {
        public SummaryPattern(string description)
        {
            Description = description;
        }

        public int NumberOfSeries { get; set; }

        public MatchResultType MatchWon { get; set; }

        public Func<int, int, SeriesScores[], bool> TeamScore { get; set; }

        public Func<int, int, bool> OpponentScore { get; set; }

        public Func<SeriesScores[], string> Commentary { get; set; }

        public string Description { get; private set; }

        public bool Matches(
            SeriesScores[] seriesScores)
        {
            int teamScore = seriesScores.Last().TeamScoreTotal;
            int opponentScore = seriesScores.Last().OpponentScoreTotal;
            MatchResultType matchWon = teamScore > opponentScore
                ? MatchResultType.Win
                : (teamScore < opponentScore ? MatchResultType.Loss : MatchResultType.Draw);
            int numberOfSeries = seriesScores.Length;

            bool matches = numberOfSeries == NumberOfSeries
                          && matchWon == MatchWon
                          && TeamScore.Invoke(teamScore, opponentScore, seriesScores)
                          && OpponentScore.Invoke(teamScore, opponentScore);

            return matches;
        }
    }
}