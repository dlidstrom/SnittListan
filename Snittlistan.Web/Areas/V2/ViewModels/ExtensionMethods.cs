﻿using System.Collections.Generic;
using System.Linq;
using Snittlistan.Web.Areas.V2.ReadModels;

namespace Snittlistan.Web.Areas.V2.ViewModels
{
    public static class ExtensionMethods
    {
        private static readonly Dictionary<char, int> TeamLevelSortOrder = new Dictionary<char, int>
            {
                { 'a', 1 },
                { 'f', 2 },
                { 'b', 3 },
                { 'c', 4 }
            };

        public static IEnumerable<RosterViewModel> SortRosters(this IEnumerable<RosterViewModel> rosters)
        {
            return rosters.OrderBy(r => TeamLevelSortOrder[r.TeamLevel]).ThenBy(r => r.Date);
        }

        public static IEnumerable<ResultHeaderReadModel> SortResults(this IEnumerable<ResultHeaderReadModel> results)
        {
            return results.OrderBy(r => TeamLevelSortOrder[r.TeamLevel]).ThenBy(r => r.Date);
        }
    }
}