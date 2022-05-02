﻿#nullable enable

using Postal;
using Snittlistan.Web.Areas.V2.Domain;

namespace Snittlistan.Web.Models;

public class UpdateRosterEmail_State : EmailState
{
    public UpdateRosterEmail_State(
        string playerEmail,
        string name,
        FormattedAuditLog formattedAuditLog,
        string[] players,
        string? teamLeader,
        string replyToEmail,
        int season,
        int turn,
        Uri rosterLink,
        Uri userProfileLink,
        bool needsAccept,
        MatchHeadType matchHead)
        : base(OwnerEmail, playerEmail, BccEmail, "Uttagning har uppdaterats")
    {
        PlayerEmail = playerEmail;
        Name = name;
        FormattedAuditLog = formattedAuditLog;
        Players = players;
        TeamLeader = teamLeader;
        ReplyToEmail = replyToEmail;
        Season = season;
        Turn = turn;
        RosterLink = rosterLink;
        UserProfileLink = userProfileLink;
        NeedsAccept = needsAccept;
        MatchHead = matchHead;
    }

    public string PlayerEmail { get; }

    public string Name { get; }

    public FormattedAuditLog FormattedAuditLog { get; }

    public string[] Players { get; }

    public string? TeamLeader { get; }

    public string ReplyToEmail { get; }

    public int Season { get; }

    public int Turn { get; }

    public Uri RosterLink { get; }

    public Uri UserProfileLink { get; }

    public bool NeedsAccept { get; }

    public MatchHeadType MatchHead { get; }

    public class MatchHeadType
    {
        public MatchHeadType(
            string firstTeamLabel,
            string homeTeamAlias,
            string secondTeamLabel,
            string awayTeamAlias,
            string hallName,
            int? oilProfileId,
            string oilProfileName,
            DateTime matchDate)
        {
            FirstTeamLabel = firstTeamLabel;
            HomeTeamAlias = homeTeamAlias;
            SecondTeamLabel = secondTeamLabel;
            AwayTeamAlias = awayTeamAlias;
            HallName = hallName;
            OilProfileId = oilProfileId;
            OilProfileName = oilProfileName;
            MatchDate = matchDate;
        }

        public string FirstTeamLabel { get; }

        public string HomeTeamAlias { get; }

        public string SecondTeamLabel { get; }

        public string AwayTeamAlias { get; }

        public string HallName { get; }

        public int? OilProfileId { get; }

        public string OilProfileName { get; }

        public DateTime MatchDate { get; }
    }

    public override Email CreateEmail()
    {
        return new UpdateRosterEmail(
            PlayerEmail,
            Name,
            FormattedAuditLog,
            Players,
            TeamLeader,
            ReplyToEmail,
            Season,
            Turn,
            RosterLink,
            UserProfileLink,
            NeedsAccept,
            MatchHead);
    }
}
