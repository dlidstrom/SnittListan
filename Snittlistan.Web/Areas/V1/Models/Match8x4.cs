﻿using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Imports.Newtonsoft.Json;

namespace Snittlistan.Web.Areas.V1.Models
{
    /// <summary>
    /// Represents a league match.
    /// </summary>
    public class Match8x4
    {
        [JsonProperty(PropertyName = "Teams")]
        private readonly List<Team8x4> teams;

        /// <summary>
        /// Initializes a new instance of the Match8x4 class.
        /// </summary>
        /// <param name="location">Match location.</param>
        /// <param name="date">Match date.</param>
        /// <param name="bitsMatchId">BITS match id.</param>
        /// <param name="teams">Teams that played the match.</param>
        [JsonConstructor]
        public Match8x4(
            string location,
            DateTimeOffset date,
            int bitsMatchId,
            IEnumerable<Team8x4> teams)
        {
            Location = location;
            Date = date;
            BitsMatchId = bitsMatchId;
            this.teams = teams.ToList();
        }

        /// <summary>
        /// Initializes a new instance of the Match8x4 class.
        /// </summary>
        /// <param name="location">Match location.</param>
        /// <param name="date">Match date.</param>
        /// <param name="bitsMatchId">BITS match id.</param>
        /// <param name="homeTeam">Home team.</param>
        /// <param name="awayTeam">Away team.</param>
        public Match8x4(
            string location,
            DateTimeOffset date,
            int bitsMatchId,
            Team8x4 homeTeam,
            Team8x4 awayTeam)
        {
            Location = location;
            Date = date;
            BitsMatchId = bitsMatchId;
            teams = new List<Team8x4> { homeTeam, awayTeam };
        }

        /// <summary>
        /// Gets or sets the match id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the match location.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the match date.
        /// </summary>
        public DateTimeOffset Date { get; set; }

        /// <summary>
        /// Gets or sets the home team.
        /// </summary>
        [JsonIgnore]
        public Team8x4 HomeTeam
        {
            get { return teams[0]; }
            set { teams[0] = value; }
        }

        /// <summary>
        /// Gets or sets the away team.
        /// </summary>
        [JsonIgnore]
        public Team8x4 AwayTeam
        {
            get { return teams[1]; }
            set { teams[1] = value; }
        }

        /// <summary>
        /// Gets or sets the BITS match id.
        /// </summary>
        public int BitsMatchId { get; set; }

        /// <summary>
        /// Gets the teams.
        /// </summary>
        public IEnumerable<Team8x4> Teams
        {
            get { return teams; }
        }
    }
}