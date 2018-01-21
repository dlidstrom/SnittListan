﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EventStoreLite;
using Raven.Imports.Newtonsoft.Json;

namespace Snittlistan.Web.Areas.V2.ReadModels
{
    public class ResultSeriesReadModel : IReadModel
    {
        public ResultSeriesReadModel()
        {
            Series = new List<Serie>();
        }

        [JsonConstructor]
        private ResultSeriesReadModel(List<Serie> series)
        {
            Series = series;
        }

        public string Id { get; set; }

        public List<Serie> Series { get; private set; }

        public static string IdFromBitsMatchId(int bitsMatchId)
        {
            return $"Series-{bitsMatchId}";
        }

        public KeyValuePair<string, List<PlayerGame>[]>[] SortedPlayers()
        {
            var first = Series.SelectMany(x => x.Tables)
                .Select(x => x.Game1);
            var second = Series.SelectMany(x => x.Tables)
                .Select(x => x.Game2);

            var combined = first.Concat(second).ToList();
            var dictionary = new Dictionary<string, List<PlayerGame>[]>();
            foreach (var game in combined)
            {
                if (dictionary.ContainsKey(game.Player) == false)
                    dictionary.Add(
                        game.Player,
                        new[]
                        {
                            new List<PlayerGame>(),
                            new List<PlayerGame>(),
                            new List<PlayerGame>(),
                            new List<PlayerGame>()
                        });
            }

            for (var i = 0; i < Series.Count; i++)
            {
                var serie = Series[i];
                foreach (var table in serie.Tables)
                {
                    var game1 = table.Game1;
                    var game2 = table.Game2;
                    dictionary[game1.Player][i].Add(new PlayerGame(game1, table.Score));
                    dictionary[game2.Player][i].Add(new PlayerGame(game2, table.Score));
                }
            }

            var q = from x in dictionary
                    let value = dictionary[x.Key]
                    let sum = value.SelectMany(y => y).Sum(z => z.Pins)
                    orderby sum descending
                    select x;
            var result = q.ToArray();
            return result;
        }

        public string SerieSum(int i)
        {
            if (Series.Count > i)
            {
                var serieSum = Series[i].Tables.Sum(t => t.Game1.Pins + t.Game2.Pins);
                return serieSum.ToString(CultureInfo.InvariantCulture);
            }

            return string.Empty;
        }

        public string Total()
        {
            if (!Series.Any()) return string.Empty;

            var total = Series.SelectMany(s => s.Tables)
                .Sum(t => t.Game1.Pins + t.Game2.Pins);
            return total.ToString(CultureInfo.InvariantCulture);
        }

        public void Clear()
        {
            Series = new List<Serie>();
        }

        public class Serie
        {
            public Serie()
            {
                Tables = new List<Table> {
                                             new Table(),
                                             new Table(),
                                             new Table(),
                                             new Table()
                                         };
            }

            [JsonConstructor]
            private Serie(List<Table> tables)
            {
                Tables = tables;
            }

            public List<Table> Tables { get; set; }

            public int TeamTotal
            {
                get { return Tables.Sum(x => x.Game1.Pins + x.Game2.Pins); }
            }
        }

        public class Table
        {
            public Table()
            {
                Game1 = new Game();
                Game2 = new Game();
            }

            [JsonConstructor]
            private Table(int score, Game game1, Game game2)
            {
                Score = score;
                Game1 = game1;
                Game2 = game2;
            }

            public int Score { get; set; }

            public Game Game1 { get; set; }

            public Game Game2 { get; set; }
        }

        public class Game
        {
            public Game()
            {
                Player = string.Empty;
            }

            public string Player { get; set; }

            public int Pins { get; set; }

            public int Strikes { get; set; }

            public int Spares { get; set; }
        }

        public class PlayerGame
        {
            public PlayerGame(Game game, int score)
            {
                Pins = game.Pins;
                Score = score;
                Strikes = game.Strikes;
                Spares = game.Spares;
            }

            public int Score { get; }

            public int Pins { get; }

            public int Strikes { get; }

            public int Spares { get; }
        }
    }
}