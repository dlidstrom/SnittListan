﻿namespace Snittlistan.Web.Areas.V2.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;

    public static class ExtensionMethods
    {
        private static readonly Dictionary<string, int> TeamLevelSortOrder = new Dictionary<string, int>
            {
                { "A", 1 },
                { "F", 2 },
                { "B", 3 },
                { "C", 4 }
            };

        public static IEnumerable<RosterViewModel> SortRosters(this IEnumerable<RosterViewModel> rosters)
        {
            return rosters.OrderBy(GetSortKey).ThenBy(r => r.Header.Date);

            (int i, string s) GetSortKey(RosterViewModel vm)
            {
                if (TeamLevelSortOrder.TryGetValue(vm.Header.TeamLevel, out int i)) return (i, string.Empty);
                return (-1, vm.Header.Team);
            }
        }

        public static IEnumerable<ResultHeaderViewModel> SortResults(this IEnumerable<ResultHeaderViewModel> results)
        {
            return results.OrderBy(GetSortKey).ThenBy(r => r.Date);

            (int i, string s) GetSortKey(ResultHeaderViewModel vm)
            {
                if (TeamLevelSortOrder.TryGetValue(vm.TeamLevel.ToUpper(), out int i)) return (i, string.Empty);
                return (-1, vm.Team);
            }
        }
    }
}
