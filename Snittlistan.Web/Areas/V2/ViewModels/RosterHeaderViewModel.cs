namespace Snittlistan.Web.Areas.V2.ViewModels
{
    using System;
    using Domain;

    public class RosterHeaderViewModel
    {
        public RosterHeaderViewModel()
        {
        }

        public RosterHeaderViewModel(
            string rosterId,
            string team,
            string teamLevel,
            string location,
            string opponent,
            DateTime date,
            OilPatternInformation oilPattern,
            string matchResultId)
        {
            RosterId = rosterId;
            Team = team;
            TeamLevel = teamLevel;
            Location = location;
            Opponent = opponent;
            Date = date;
            OilPattern = oilPattern;
            MatchResultId = matchResultId;
        }

        public string RosterId { get; set; }

        public string Team { get; set; }

        public string TeamLevel { get; set; }

        public string Location { get; set; }

        public string Opponent { get; set; }

        public DateTime Date { get; set; }

        public OilPatternInformation OilPattern { get; set; }

        public string MatchResultId { get; set; }
    }
}