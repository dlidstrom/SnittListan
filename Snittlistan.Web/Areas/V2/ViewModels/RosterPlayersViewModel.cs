namespace Snittlistan.Web.Areas.V2.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class RosterPlayersViewModel
    {
        [Required(ErrorMessage = "Ange namn p� spelare 1")]
        public string Table1Player1 { get; set; }

        [Required(ErrorMessage = "Ange namn p� spelare 2")]
        public string Table1Player2 { get; set; }

        [Required(ErrorMessage = "Ange namn p� spelare 3")]
        public string Table2Player1 { get; set; }

        [Required(ErrorMessage = "Ange namn p� spelare 4")]
        public string Table2Player2 { get; set; }

        [Required(ErrorMessage = "Ange namn p� spelare 5")]
        public string Table3Player1 { get; set; }

        [Required(ErrorMessage = "Ange namn p� spelare 6")]
        public string Table3Player2 { get; set; }

        [Required(ErrorMessage = "Ange namn p� spelare 7")]
        public string Table4Player1 { get; set; }

        [Required(ErrorMessage = "Ange namn p� spelare 8")]
        public string Table4Player2 { get; set; }

        public string Reserve { get; set; }
    }
}