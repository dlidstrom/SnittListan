﻿namespace Snittlistan.Models
{
	using System.Collections.Generic;
	using System.Linq;
	using Newtonsoft.Json;

	/// <summary>
	/// Represents a team in a match.
	/// </summary>
	public class Team
	{
		private static Dictionary<int, int[]> homeScheme = new Dictionary<int, int[]>
		{
			{ 0, new int[] { 0, 2, 3, 1 } },
			{ 1, new int[] { 1, 3, 2, 0 } },
			{ 2, new int[] { 2, 0, 1, 3 } },
			{ 3, new int[] { 3, 1, 0, 2 } },
		};

		private static Dictionary<int, int[]> awayScheme = new Dictionary<int, int[]>
		{
			{ 0, new int[] { 0, 3, 1, 2 } },
			{ 1, new int[] { 1, 2, 0, 3 } },
			{ 2, new int[] { 2, 1, 3, 0 } },
			{ 3, new int[] { 3, 0, 2, 1 } },
		};

		[JsonProperty(PropertyName = "Series")]
		private List<Serie> series;

		/// <summary>
		/// Initializes a new instance of the Team class.
		/// </summary>
		/// <param name="name">Name of the team.</param>
		/// <param name="score">Total score.</param>
		public Team(string name, int score)
		{
			Name = name;
			Score = score;
			series = new List<Serie>();
		}

		/// <summary>
		/// Initializes a new instance of the Team class.
		/// </summary>
		/// <param name="name">Name of the team.</param>
		/// <param name="score">Total score.</param>
		/// <param name="series">Series played by team.</param>
		[JsonConstructor]
		public Team(string name, int score, IEnumerable<Serie> series)
		{
			Name = name;
			Score = score;
			this.series = new List<Serie>(series);
		}

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the total score.
		/// </summary>
		public int Score { get; set; }

		/// <summary>
		/// Gets the series.
		/// </summary>
		public IEnumerable<Serie> Series
		{
			get { return series; }
		}

		[JsonProperty]
		private bool HomeTeam { get; set; }

		/// <summary>
		/// H1-B1 H2-B2 H3-B3 H4-B4
		/// H3-B4 H4-B3 H1-B2 H2-B1
		/// H4-B2 H3-B1 H2-B4 H1-B3
		/// H2-B3 H1-B4 H4-B1 H3-B2.
		/// </summary>
		/// <param name="name">Name of team.</param>
		/// <param name="score">Team score.</param>
		/// <param name="series">Team series.</param>
		public static Team CreateHomeTeam(string name, int score, IEnumerable<Serie> series)
		{
			var serie1tables = series.ElementAt(0).Tables;
			var serie2tables = series.ElementAt(1).Tables;
			var serie3tables = series.ElementAt(2).Tables;
			var serie4tables = series.ElementAt(3).Tables;
			var seriesInOrder = new List<Serie>
			{
				CreateSerie(serie1tables, 0, 1, 2, 3),
				CreateSerie(serie2tables, 2, 3, 0, 1),
				CreateSerie(serie3tables, 3, 2, 1, 0),
				CreateSerie(serie4tables, 1, 0, 3, 2)
			};

			return new Team(name, score, seriesInOrder) { HomeTeam = true };
		}

		/// <summary>
		/// H1-B1 H2-B2 H3-B3 H4-B4
		/// H3-B4 H4-B3 H1-B2 H2-B1
		/// H4-B2 H3-B1 H2-B4 H1-B3
		/// H2-B3 H1-B4 H4-B1 H3-B2.
		/// </summary>
		/// <param name="name">Name of team.</param>
		/// <param name="score">Team score.</param>
		/// <param name="series">Team series.</param>
		public static Team CreateAwayTeam(string name, int score, IEnumerable<Serie> series)
		{
			var serie1tables = series.ElementAt(0).Tables;
			var serie2tables = series.ElementAt(1).Tables;
			var serie3tables = series.ElementAt(2).Tables;
			var serie4tables = series.ElementAt(3).Tables;
			var seriesInOrder = new List<Serie>
			{
				CreateSerie(serie1tables, 0, 1, 2, 3),
				CreateSerie(serie2tables, 3, 2, 1, 0),
				CreateSerie(serie3tables, 1, 0, 3, 2),
				CreateSerie(serie4tables, 2, 3, 0, 1)
			};

			return new Team(name, score, seriesInOrder) { HomeTeam = false };
		}

		/// <summary>
		/// Returns the total pins.
		/// </summary>
		/// <returns>Total pins.</returns>
		public int Pins()
		{
			return Series.Sum(s => s.Pins());
		}

		/// <summary>
		/// Returns the score for a serie.
		/// </summary>
		/// <param name="serie">Serie index (1-based).</param>
		/// <returns></returns>
		public int ScoreFor(int serie)
		{
			return Series.ElementAt(serie - 1).Score();
		}

		/// <summary>
		/// Returns the total pins for a serie.
		/// </summary>
		/// <param name="serie">Serie number (1-based).</param>
		/// <returns>Total pins for the specified serie.</returns>
		public int PinsFor(int serie)
		{
			return Series.ElementAt(serie - 1).Pins();
		}

		/// <summary>
		/// Returns the total pins for a player.
		/// </summary>
		/// <param name="player">Player name.</param>
		/// <returns>Total pins for player in all series.</returns>
		public int PinsForPlayer(string player)
		{
			return Series.Sum(s => s.PinsForPlayer(player));
		}

		/// <summary>
		/// H1-B1 H2-B2 H3-B3 H4-B4
		/// H3-B4 H4-B3 H1-B2 H2-B1
		/// H4-B2 H3-B1 H2-B4 H1-B3
		/// H2-B3 H1-B4 H4-B1 H3-B2.
		/// </summary>
		/// <param name="serie">Serie to fetch.</param>
		/// <param name="pair">Pair to fetch.</param>
		/// <returns>Serie for pair.</returns>
		public Table TableAt(int serie, int pair)
		{
			if (HomeTeam)
				return series[serie].Tables.ElementAt(homeScheme[pair][serie]);
			else
				return series[serie].Tables.ElementAt(awayScheme[pair][serie]);
		}

		private static Serie CreateSerie(IEnumerable<Table> tables, int i1, int i2, int i3, int i4)
		{
			return new Serie(new List<Table>
			{
				tables.ElementAt(i1),
				tables.ElementAt(i2),
				tables.ElementAt(i3),
				tables.ElementAt(i4)
			});
		}
	}
}