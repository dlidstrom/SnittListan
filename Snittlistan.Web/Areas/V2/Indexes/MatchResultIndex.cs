namespace Snittlistan.Web.Areas.V2.Indexes
{
    using System;
    using System.Linq;
    using Raven.Abstractions.Indexing;
    using Raven.Client.Indexes;
    using Snittlistan.Web.Areas.V2.Models;

    public class MatchResultIndex : AbstractIndexCreationTask<MatchResult, MatchResultIndex.Result>
    {
        public MatchResultIndex()
        {
            this.Map = results => from result in results
                                  let roster = this.LoadDocument<Roster>(result.RosterId)
                                  select new
                                  {
                                      result.RosterId,
                                      roster.Turn,
                                      roster.Season,
                                      roster.Date,
                                      roster.Team,
                                      roster.Opponent,
                                      roster.Location,
                                      result.TeamScore,
                                      result.OpponentScore,
                                      result.BitsMatchId
                                  };

            this.Store(x => x.Turn, FieldStorage.Yes);
            this.Store(x => x.Season, FieldStorage.Yes);
            this.Store(x => x.Date, FieldStorage.Yes);
            this.Store(x => x.Team, FieldStorage.Yes);
            this.Store(x => x.Opponent, FieldStorage.Yes);
            this.Store(x => x.Location, FieldStorage.Yes);
        }

        public class Result
        {
            public string RosterId { get; set; }
            public int Turn { get; set; }
            public int Season { get; set; }
            public DateTimeOffset Date { get; set; }
            public string Team { get; set; }
            public string Opponent { get; set; }
            public string Location { get; set; }
            public int TeamScore { get; set; }
            public int OpponentScore { get; set; }
            public int BitsMatchId { get; set; }
        }
    }
}