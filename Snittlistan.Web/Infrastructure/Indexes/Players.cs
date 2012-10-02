﻿namespace Snittlistan.Web.Infrastructure.Indexes
{
    using System.Linq;

    using Raven.Client.Indexes;

    using Snittlistan.Web.Models;

    public class Players : AbstractMultiMapIndexCreationTask<Players.Result>
    {
        public Players()
        {
            this.AddMap<Match8x4>(matches => from match in matches
                                             from team in match.Teams
                                             from serie in team.Series
                                             from table in serie.Tables
                                             from game in table.Games
                                             select new { game.Player });

            this.AddMap<Match4x4>(matches => from match in matches
                                             from team in match.Teams
                                             from serie in team.Series
                                             from game in serie.Games
                                             select new { game.Player });

            this.Reduce = results => from result in results
                                     group result by result.Player into g
                                     select new Result { Player = g.Key };
        }

        public class Result
        {
            public string Player { get; set; }
        }
    }
}