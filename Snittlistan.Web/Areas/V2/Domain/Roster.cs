﻿namespace Snittlistan.Web.Areas.V2.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Raven.Abstractions;
    using Raven.Client;

    public class Roster : IAuditLogCapable
    {
        private string teamLevel;
        private List<string> acceptedPlayers;

        public Roster(
            int season,
            int turn,
            int bitsMatchId,
            string team,
            string teamLevel,
            string location,
            string opponent,
            DateTime date,
            bool isFourPlayer,
            OilPatternInformation oilPattern,
            List<AuditLogEntry> auditLogEntries = null)
        {
            Season = season;
            Turn = turn;
            BitsMatchId = bitsMatchId;
            Team = team;
            TeamLevel = teamLevel ?? Team.Substring(team.Length - 1);
            Location = location;
            Opponent = opponent;
            Date = date;
            IsFourPlayer = isFourPlayer;
            OilPattern = oilPattern ?? new OilPatternInformation(string.Empty, string.Empty);
            AuditLogEntries = auditLogEntries ?? new List<AuditLogEntry>();
        }

        public string Id { get; set; }

        public int Season { get; set; }

        public int Turn { get; set; }

        public int BitsMatchId { get; set; }

        public string Team { get; set; }

        // TODO: should this be calculated from Team?
        // Team.Substring(roster.Team.LastIndexOf(' ') + 1)
        public string TeamLevel
        {
            get { return teamLevel; }
            set { teamLevel = value.Trim(); }
        }

        public string Location { get; set; }

        public string Opponent { get; set; }

        public DateTime Date { get; set; }

        public bool IsFourPlayer { get; set; }

        public OilPatternInformation OilPattern { get; set; }
        public List<AuditLogEntry> AuditLogEntries { get; }
        public bool Preliminary { get; set; }

        public List<string> Players { get; set; } = new List<string>();

        public void SetPlayers(List<string> players)
        {
            Players = players;
            AcceptedPlayers.RemoveAll(x => players.Contains(x) == false);
        }

        public string MatchResultId { get; set; }

        public string TeamLeader { get; set; }

        public bool IsVerified { get; set; }

        public List<string> AcceptedPlayers
        {
            get { return acceptedPlayers ?? new List<string>(); }
            set { acceptedPlayers = value; }
        }

        public void UpdateWith(Update update)
        {
            var change = new Change(update.ChangeType, update.UserId);
            object before = GetState();

            update.PlayerAccepted.Match(
                x =>
                {
                    change.PlayerAccepted = AuditLogEntry.PropertyChange.Create(null, x);
                    Accept(x);
                },
                () => { });

            update.OilPattern.Match(
                x =>
                {
                    if (x != OilPattern)
                    {
                        change.OilPattern = AuditLogEntry.PropertyChange.Create(OilPattern, x);
                        OilPattern = x;
                    }
                },
                () => { });

            update.Date.Match(
                x =>
                {
                    if (x != Date)
                    {
                        change.Date = AuditLogEntry.PropertyChange.Create(Date, x);
                        Date = x;
                    }
                },
                () => { });

            update.Opponent.Match(
                x =>
                {
                    if (x != Opponent)
                    {
                        change.Opponent = AuditLogEntry.PropertyChange.Create(Opponent, x);
                        Opponent = x;
                    }
                },
                () => { });

            update.Location.Match(
                x =>
                {
                    if (x != Location)
                    {
                        change.Location = AuditLogEntry.PropertyChange.Create(Location, x);
                        Location = x;
                    }
                },
                () => { });

            update.Players.Match(
                x =>
                {
                    if (x.SequenceEqual(Players) == false)
                    {
                        change.Players = AuditLogEntry.PropertyChange.Create(Players, x);
                        Players = x;
                    }
                },
                () => { });

            update.IsVerified.Match(
                x =>
                {
                    if (x != IsVerified)
                    {
                        change.IsVerified = AuditLogEntry.PropertyChange.Create(IsVerified, x);
                        IsVerified = x;
                    }
                },
                () => { });

            object after = GetState();
            var auditLogEntry = new AuditLogEntry(
                change.UserId,
                change.ChangeType.ToString(),
                change,
                before,
                after);
            AuditLogEntries.Add(auditLogEntry);
        }

        private void Accept(string playerId)
        {
            if (playerId == null) throw new ArgumentNullException(nameof(playerId));
            if (Preliminary) throw new Exception("Can not accept when preliminary");
            if (SystemTime.UtcNow > Date.ToUniversalTime()) throw new Exception("Can not accept passed games");
            if (Players.Contains(playerId) == false) throw new Exception("Can only accept players on the roster");
            AcceptedPlayers = new HashSet<string>(AcceptedPlayers.Concat(new[] { playerId })).ToList();
        }

        private object GetState()
        {
            return new
            {
                Players = Players.ToArray(),
                AcceptedPlayers = AcceptedPlayers.ToArray()
            };
        }

        public FormattedAuditLogEntry[] GetHistory(IDocumentSession documentSession)
        {
            return AuditLogEntries.Select(Format).ToArray();

            FormattedAuditLogEntry Format(AuditLogEntry entry)
            {
                var change = (Change)entry.Change;
                string action = null;
                switch (change.ChangeType)
                {
                    case ChangeType.PlayerAccepted:
                        {
                            Player player = documentSession.Load<Player>(change.PlayerAccepted.NewValue);
                            action = $"{player.Name} accepterade";
                            break;
                        }
                    case ChangeType.VerifyMatchMessage:
                        {
                            action = "Verifierade resultatet";
                            break;
                        }
                }

                return new FormattedAuditLogEntry(
                    GetUser(entry.UserId),
                    action,
                    entry.Date);

                string GetUser(string userId)
                {
                    Player player = documentSession.Load<Player>(entry.UserId);
                    return player?.Name ?? userId;
                }
            }
        }

        public class Change
        {
            public Change(
                ChangeType changeType,
                string userId)
            {
                ChangeType = changeType;
                UserId = userId;
            }

            public ChangeType ChangeType { get; }
            public string UserId { get; }
            public AuditLogEntry.PropertyChange<string> PlayerAccepted { get; set; }
            public AuditLogEntry.PropertyChange<OilPatternInformation> OilPattern { get; set; }
            public AuditLogEntry.PropertyChange<DateTime> Date { get; set; }
            public AuditLogEntry.PropertyChange<string> Opponent { get; set; }
            public AuditLogEntry.PropertyChange<string> Location { get; set; }
            public AuditLogEntry.PropertyChange<List<string>> Players { get; set; }
            public AuditLogEntry.PropertyChange<bool> IsVerified { get; set; }
        }

        public class Update
        {
            public Update(
                ChangeType changeType,
                string userId)
            {
                ChangeType = changeType;
                UserId = userId;
            }

            public ChangeType ChangeType { get; }
            public string UserId { get; }
            public Option<string> PlayerAccepted { get; set; } = None.Value;
            public Option<OilPatternInformation> OilPattern { get; set; } = None.Value;
            public Option<DateTime> Date { get; set; } = None.Value;
            public Option<string> Opponent { get; set; } = None.Value;
            public Option<string> Location { get; set; } = None.Value;
            public Option<List<string>> Players { get; set; } = None.Value;
            public Option<bool> IsVerified { get; set; } = None.Value;
        }

        public enum ChangeType
        {
            PlayerAccepted,
            VerifyMatchMessage
        }
    }
}
