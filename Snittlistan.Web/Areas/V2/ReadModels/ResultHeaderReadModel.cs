﻿using System;
using EventStoreLite;
using Raven.Imports.Newtonsoft.Json;
using Snittlistan.Web.Areas.V2.Domain;

namespace Snittlistan.Web.Areas.V2.ReadModels
{
    // TODO Store only roster id. Use index to read and display roster values
    public class ResultHeaderReadModel : IReadModel
    {
        public static string IdFromBitsMatchId(int bitsMatchId)
        {
            return "ResultHeader-" + bitsMatchId;
        }

        [JsonConstructor]
        private ResultHeaderReadModel()
        {
        }

        public ResultHeaderReadModel(
            Roster roster,
            string aggregateId,
            int teamScore,
            int opponentScore,
            int bitsMatchId)
        {
            Id = IdFromBitsMatchId(bitsMatchId);
            this.SetValues(roster, aggregateId, teamScore, opponentScore, bitsMatchId);
        }

        public void SetValues(Roster roster, string aggregateId, int teamScore, int opponentScore, int bitsMatchId)
        {
            this.Season = roster.Season;
            this.Turn = roster.Turn;
            this.AggregateId = aggregateId;
            this.RosterId = roster.Id;
            this.Date = roster.Date;
            this.Location = roster.Location;
            this.TeamLevel = char.ToLower(roster.Team[roster.Team.Length - 1]);
            this.Team = roster.Team;
            this.Opponent = roster.Opponent;
            this.TeamScore = teamScore;
            this.OpponentScore = opponentScore;
            this.BitsMatchId = bitsMatchId;
        }

        /// <summary>
        /// The BITS match id.
        /// </summary>
        public string Id { get; private set; }

        public int Season { get; private set; }

        public int Turn { get; private set; }

        public string AggregateId { get; private set; }

        public string RosterId { get; private set; }

        public DateTimeOffset Date { get; private set; }

        public string Location { get; private set; }

        public char TeamLevel { get; private set; }

        public string Team { get; private set; }

        public string Opponent { get; private set; }

        public int TeamScore { get; private set; }

        public int OpponentScore { get; private set; }

        public int BitsMatchId { get; private set; }
    }
}