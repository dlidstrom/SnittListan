﻿namespace Snittlistan.Web.ViewModels.Match
{
    using System.Web.Mvc;

    public class EditTeam4x4ViewModel
    {
        [HiddenInput]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is the home team.
        /// Used when updating match.
        /// </summary>
        [HiddenInput]
        public bool IsHomeTeam { get; set; }

        /// <summary>
        /// Gets or sets the team.
        /// </summary>
        public Team4x4ViewModel Team { get; set; }
    }
}