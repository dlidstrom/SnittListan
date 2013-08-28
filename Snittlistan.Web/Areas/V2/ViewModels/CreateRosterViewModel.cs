using System;
using System.ComponentModel.DataAnnotations;
using Raven.Abstractions;

namespace Snittlistan.Web.Areas.V2.ViewModels
{
    public class CreateRosterViewModel
    {
        public CreateRosterViewModel()
        {
            Team = string.Empty;
            Location = string.Empty;
            Opponent = string.Empty;
            Date = SystemTime.UtcNow.ToLocalTime().Date;
            Time = "10:00";
        }

        [Required]
        public int Season { get; set; }

        [Required]
        public int Turn { get; set; }

        [Required]
        public string Team { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string Opponent { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [RegularExpression(@"^(?:0?[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Invalid time.")]
        public string Time { get; set; }

        public bool IsFourPlayer { get; set; }
    }
}