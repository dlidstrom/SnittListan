﻿namespace Snittlistan.Web.Areas.V2.Indexes
{
    using System;
    using System.Linq;
    using Raven.Abstractions.Indexing;
    using Raven.Client.Indexes;
    using Snittlistan.Web.Areas.V2.Domain;

    public class RosterSearchTerms : AbstractIndexCreationTask<Roster, RosterSearchTerms.Result>
    {
        public RosterSearchTerms()
        {
            Map = rosters => from roster in rosters
                             select new
                             {
                                 roster.Id,
                                 roster.Team,
                                 roster.Opponent,
                                 roster.Location,
                                 roster.Turn,
                                 roster.BitsMatchId,
                                 roster.Season,
                                 roster.Date,
                                 roster.MatchResultId,
                                 roster.Preliminary,
                                 PlayerCount = roster.Players.Count,
                                 OilPatternName = roster.OilPattern.Name
                             };

            Store(x => x.Id, FieldStorage.Yes);
            Store(x => x.PlayerCount, FieldStorage.Yes);
            Store(x => x.OilPatternName, FieldStorage.Yes);
        }

        public class Result
        {
            public string Id { get; set; }

            public string Team { get; set; }

            public string Opponent { get; set; }

            public string Location { get; set; }

            public int Turn { get; set; }

            public int BitsMatchId { get; set; }

            public int Season { get; set; }

            public DateTime Date { get; set; }

            public string MatchResultId { get; set; }

            public bool Preliminary { get; set; }

            public int PlayerCount { get; set; }

            public string OilPatternName { get; set; }
        }
    }
}