﻿#nullable enable

namespace Snittlistan.Web.Areas.V2.ReadModels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using EventStoreLite;
    using Snittlistan.Web.Areas.V2.Domain;
    using Snittlistan.Web.Areas.V2.Domain.Match;

    public class SeasonResults : IReadModel
    {
        public SeasonResults(int season)
        {
            Id = GetId(season);
            PlayerResults = new HashSet<PlayerResult>();
        }

        public string Id { get; private set; }

        public HashSet<PlayerResult> PlayerResults { get; private set; }

        public static string GetId(int season)
        {
            return "SeasonResults-" + season;
        }

        public void Add(int bitsMatchId, string rosterId, DateTime date, int turn, MatchSerie matchSerie)
        {
            foreach (MatchTable matchTable in new[] { matchSerie.Table1, matchSerie.Table2, matchSerie.Table3, matchSerie.Table4 })
            {
                PlayerResult firstPlayerResult = new(
                    bitsMatchId,
                    rosterId,
                    date,
                    turn,
                    matchSerie.SerieNumber,
                    matchTable.TableNumber,
                    matchTable.Game1.Player,
                    matchTable.Score,
                    matchTable.Game1.Pins);
                _ = PlayerResults.Add(firstPlayerResult);
                PlayerResult secondPlayerResult = new(
                    bitsMatchId,
                    rosterId,
                    date,
                    turn,
                    matchSerie.SerieNumber,
                    matchTable.TableNumber,
                    matchTable.Game2.Player,
                    matchTable.Score,
                    matchTable.Game2.Pins);
                _ = PlayerResults.Add(secondPlayerResult);
            }
        }

        public void Add(int bitsMatchId, string rosterId, DateTime date, int turn, MatchSerie4 matchSerie)
        {
            (int tableNumber, MatchGame4 game)[] games = new[]
            {
                (tableNumber: 1, game: matchSerie.Game1),
                (tableNumber: 2, game: matchSerie.Game2),
                (tableNumber: 3, game: matchSerie.Game3),
                (tableNumber: 4, game: matchSerie.Game4)
            };
            foreach ((int tableNumber, MatchGame4 game) in games)
            {
                PlayerResult playerResult = new(
                    bitsMatchId,
                    rosterId,
                    date,
                    turn,
                    matchSerie.SerieNumber,
                    tableNumber,
                    game.Player,
                    game.Score,
                    game.Pins);
                _ = PlayerResults.Add(playerResult);
            }
        }

        public HashSet<Tuple<PlayerResult, bool>> GetTopThreeResults(string playerId, EliteMedals.EliteMedal.EliteMedalValue existingMedal)
        {
            var query =
                from playerResult in PlayerResults
                where playerResult.PlayerId == playerId
                group playerResult by new
                {
                    playerResult.BitsMatchId,
                    playerResult.RosterId
                }
                into grouping
                where grouping.Count() == 4
                let totalPins = grouping.Sum(x => x.Pins)
                let validResult = (existingMedal == EliteMedals.EliteMedal.EliteMedalValue.None && totalPins >= 760)
                    || (existingMedal == EliteMedals.EliteMedal.EliteMedalValue.Bronze && totalPins >= 800)
                    || totalPins >= 840
                orderby totalPins descending
                select new
                {
                    grouping.Key.BitsMatchId,
                    grouping.Key.RosterId,
                    PlayerResults = grouping.ToArray(),
                    ValidResult = validResult
                };
            var topThree = query.Take(3).ToArray();
            Tuple<PlayerResult, bool>[] playerResults = topThree.SelectMany(x => x.PlayerResults.Select(y => Tuple.Create(y, x.ValidResult))).ToArray();
            HashSet<Tuple<PlayerResult, bool>> topThreeResults = new(playerResults);
            return topThreeResults;
        }

        public void RemoveWhere(int bitsMatchId, string rosterId)
        {
            if (bitsMatchId != 0)
            {
                _ = PlayerResults.RemoveWhere(x => x.BitsMatchId == bitsMatchId);
            }
            else
            {
                _ = PlayerResults.RemoveWhere(x => x.RosterId == rosterId);
            }
        }

        [DebuggerDisplay("BitsMatchId={BitsMatchId} RosterId={RosterId} Date={Date} Turn={Turn} SerieNumber={SerieNumber} TableNumber={TableNumber} PlayerId={PlayerId} Score={Score} Pins={Pins}")]
        public class PlayerResult
        {
            public PlayerResult(int bitsMatchId, string rosterId, DateTime date, int turn, int serieNumber, int tableNumber, string playerId, int score, int pins)
            {
                BitsMatchId = bitsMatchId;
                RosterId = rosterId;
                Date = date;
                Turn = turn;
                SerieNumber = serieNumber;
                TableNumber = tableNumber;
                PlayerId = playerId;
                Score = score;
                Pins = pins;
            }

            public int BitsMatchId { get; }

            public string RosterId { get; }

            public DateTime Date { get; }

            public int Turn { get; }

            public int SerieNumber { get; }

            public int TableNumber { get; }

            public string PlayerId { get; }

            public int Score { get; }

            public int Pins { get; }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = BitsMatchId;
                    hashCode = (hashCode * 397) ^ (RosterId != null ? RosterId.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ SerieNumber;
                    hashCode = (hashCode * 397) ^ TableNumber;
                    hashCode = (hashCode * 397) ^ (PlayerId != null ? PlayerId.GetHashCode() : 0);
                    return hashCode;
                }
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as PlayerResult);
            }

            public bool Equals(PlayerResult? playerResult)
            {
                bool eq = playerResult?.BitsMatchId == BitsMatchId
                    && string.Equals(playerResult.RosterId, RosterId)
                    && playerResult.SerieNumber == SerieNumber
                    && playerResult.TableNumber == TableNumber
                    && string.Equals(playerResult.PlayerId, PlayerId);
                return eq;
            }
        }
    }
}
