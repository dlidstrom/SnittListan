﻿using System.Collections.Generic;
using System.Linq;
using Snittlistan.Web.Areas.V2.ReadModels;

namespace Snittlistan.Web.Areas.V2.ViewModels
{
    public class TeamOfWeekLeadersViewModel
    {
        public TeamOfWeekLeadersViewModel(IEnumerable<TeamOfWeek> weeks)
        {
            var rankByTurn = new Dictionary<int, List<PlayerScore>>();
            foreach (var week in weeks)
            {
                List<PlayerScore> list;
                if (rankByTurn.TryGetValue(week.Turn, out list) == false)
                {
                    rankByTurn[week.Turn] = week.PlayerScores.Values.ToList();
                }
                else
                {
                    foreach (var value in week.PlayerScores.Values)
                    {
                        var item = list.SingleOrDefault(x => x.PlayerId == value.PlayerId);
                        if (item == null)
                            list.Add(value);
                        else if (item.Pins < value.Pins)
                        {
                            list.Remove(item);
                            list.Add(value);
                        }
                    }
                }
            }

            var bestOfBest = new List<string>();
            foreach (var turn in rankByTurn.Keys)
            {
                var current = -1;
                var rank = 1;
                foreach (var playerScore in rankByTurn[turn].OrderByDescending(x => x.Pins))
                {
                    if (playerScore.Pins != current)
                    {
                        rank++;
                        current = playerScore.Pins;
                    }

                    if (rank > 9) break;

                    bestOfBest.Add(playerScore.Name);
                }
            }

            Top9Total = bestOfBest.GroupBy(x => x)
                                  .Select(x => new NameCount(x))
                                  .OrderByDescending(x => x.Count)
                                  .ThenBy(x => x.Name)
                                  .ToArray();
        }

        public NameCount[] Top9Total { get; private set; }

        public class NameCount
        {
            public NameCount(IGrouping<string, string> grouping)
            {
                Name = grouping.Key;
                Count = grouping.Count();
            }

            public string Name { get; }

            public int Count { get; }
        }
    }
}