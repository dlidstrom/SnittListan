﻿#nullable enable

namespace Snittlistan.Web.Infrastructure.Indexes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Raven.Abstractions.Indexing;
    using Raven.Client.Indexes;
    using Snittlistan.Web.Areas.V1.Models;

    public class Match_ByDate : AbstractMultiMapIndexCreationTask<Match_ByDate.Result>
    {
        public Match_ByDate()
        {
            AddMap<Match4x4>(matches => from match in matches
                                        select new
                                        {
                                            match.Id,
                                            Type = "4x4",
                                            match.Date,
                                            match.Location,
                                            HomeTeamName = match.Teams.ElementAt(0).Name,
                                            HomeTeamScore = match.Teams.ElementAt(0).Score,
                                            AwayTeamName = match.Teams.ElementAt(1).Name,
                                            AwayTeamScore = match.Teams.ElementAt(1).Score
                                        });

            AddMap<Match8x4>(matches => from match in matches
                                        select new
                                        {
                                            match.Id,
                                            Type = "8x4",
                                            match.Date,
                                            match.Location,
                                            HomeTeamName = match.Teams.ElementAt(0).Name,
                                            HomeTeamScore = match.Teams.ElementAt(0).Score,
                                            AwayTeamName = match.Teams.ElementAt(1).Name,
                                            AwayTeamScore = match.Teams.ElementAt(1).Score
                                        });

            Store(x => x.Id, FieldStorage.Yes);
            Store(x => x.Type, FieldStorage.Yes);
            Store(x => x.HomeTeamName, FieldStorage.Yes);
            Store(x => x.HomeTeamScore, FieldStorage.Yes);
            Store(x => x.AwayTeamName, FieldStorage.Yes);
            Store(x => x.AwayTeamScore, FieldStorage.Yes);
        }

        public class Result
        {
            private string id = null!;

            public string Id
            {
                get => id;

                set => id = value.Substring(value.LastIndexOf('-') + 1);
            }

            public string Type { get; set; } = null!;

            [Display(Name = "Datum"), DataType(DataType.Date)]
            public DateTimeOffset Date { get; set; }

            public string HomeTeamName { get; set; } = null!;

            public int HomeTeamScore { get; set; }

            public string AwayTeamName { get; set; } = null!;

            public int AwayTeamScore { get; set; }

            public string Location { get; set; } = null!;
        }
    }
}
