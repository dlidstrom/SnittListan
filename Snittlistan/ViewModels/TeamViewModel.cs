using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Snittlistan.ViewModels
{
	/// <summary>
	/// Represents a team.
	/// </summary>
	public class TeamViewModel
	{
		/// <summary>
		/// Initializes a new instance of the TeamViewModel class.
		/// </summary>
		public TeamViewModel()
		{
			Serie1 = new Serie();
			Serie2 = new Serie();
			Serie3 = new Serie();
			Serie4 = new Serie();
		}

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		[Display(Name = "Namn")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the score.
		/// </summary>
		[Display(Name = "Banpoäng"), Range(0, 20)]
		public int Score { get; set; }

		/// <summary>
		/// Gets or sets the first serie.
		/// </summary>
		[Display(Name = "Serie 1")]
		public Serie Serie1 { get; set; }

		/// <summary>
		/// Gets or sets the second serie.
		/// </summary>
		[Display(Name = "Serie 2")]
		public Serie Serie2 { get; set; }

		/// <summary>
		/// Gets or sets the third serie.
		/// </summary>
		[Display(Name = "Serie 3")]
		public Serie Serie3 { get; set; }

		/// <summary>
		/// Gets or sets the fourth serie.
		/// </summary>
		[Display(Name = "Serie 4")]
		public Serie Serie4 { get; set; }

		/// <summary>
		/// Represents a serie in a match.
		/// </summary>
		public class Serie
		{
			/// <summary>
			/// Initializes a new instance of the Serie class.
			/// </summary>
			public Serie()
			{
				Table1 = new Table();
				Table2 = new Table();
				Table3 = new Table();
				Table4 = new Table();
			}

			/// <summary>
			/// Gets or sets table 1.
			/// </summary>
			public Table Table1 { get; set; }

			/// <summary>
			/// Gets or sets table 2.
			/// </summary>
			public Table Table2 { get; set; }

			/// <summary>
			/// Gets or sets table 3.
			/// </summary>
			public Table Table3 { get; set; }

			/// <summary>
			/// Gets or sets table 4.
			/// </summary>
			public Table Table4 { get; set; }
		}

		/// <summary>
		/// Represents a table in a serie.
		/// </summary>
		public class Table
		{
			/// <summary>
			/// Initializes a new instance of the Table class.
			/// </summary>
			public Table()
			{
				Game1 = new Game();
				Game2 = new Game();
			}

			/// <summary>
			/// Gets or sets the score.
			/// </summary>
			[Display(Name = "Banpoäng")]
			public int Score { get; set; }

			/// <summary>
			/// Gets or sets the first game.
			/// </summary>
			public Game Game1 { get; set; }

			/// <summary>
			/// Gets or sets the second game.
			/// </summary>
			public Game Game2 { get; set; }
		}
		
		/// <summary>
		/// Represents a game in a table.
		/// </summary>
		public class Game
		{
			/// <summary>
			/// Initializes a new instance of the Game class.
			/// </summary>
			public Game()
			{
				Player = string.Empty;
			}

			/// <summary>
			/// Gets or sets the player name.
			/// </summary>
			[Display(Name = "Spelare")]
			public string Player { get; set; }

			/// <summary>
			/// Gets or sets the number of pins.
			/// </summary>
			[Display(Name = "Kägelpoäng")]
			public int Pins { get; set; }

			/// <summary>
			/// Gets or sets the number of strikes.
			/// </summary>
			[Display(Name = "X")]
			public int Strikes { get; set; }

			/// <summary>
			/// Gets or sets the number of misses.
			/// </summary>
			[Display(Name = "Miss")]
			public int Misses { get; set; }

			/// <summary>
			/// Gets or sets the number of one-pin misses.
			/// </summary>
			[Display(Name = "9-")]
			public int OnePinMisses { get; set; }

			/// <summary>
			/// Gets or sets the number of splits.
			/// </summary>
			[Display(Name = "Hål")]
			public int Splits { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether all frames were covered.
			/// </summary>
			[Display(Name = "Alla täckta")]
			public bool CoveredAll { get; set; }
		}
	}
}