﻿namespace Snittlistan.Infrastructure.Indexes
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.Linq;
	using Raven.Client.Indexes;
	using Snittlistan.Models;

    public class Player_ByMatch : AbstractIndexCreationTask<Match, Player_ByMatch.Result>
    {
        public Player_ByMatch()
        {
            Map = matches => from match in matches
                             from team in match.Teams
                             from serie in team.Series
                             from table in serie.Tables
                             from game in table.Games
                             select new
                             {
								 MatchId = match.Id,
								 Player = game.Player,
								 Location = match.Location,
								 Team = team.Name,
                                 Date = match.Date,
                                 Pins = game.Pins,
								 Series = 1
                             };

			Reduce = results => from result in results
								group result by new { result.MatchId, result.Player, result.Location, result.Date, result.Team } into games
								select new
								{
									MatchId = games.Key.MatchId,
									Player = games.Key.Player,
									Location = games.Key.Location,
									Team = games.Key.Team,
									Date = games.Key.Date,
									Pins = games.Sum(g => g.Pins),
									Series = games.Sum(g => g.Series)
								};
        }

        public class Result
        {
            public string Player { get; set; }
			public string MatchId { get; set; }
			public string Location { get; set; }
			public string Team { get; set; }

			[DataType(DataType.Date)]
			public DateTime Date { get; set; }
			public double Pins { get; set; }
			public int Series { get; set; }
        }
    }
}