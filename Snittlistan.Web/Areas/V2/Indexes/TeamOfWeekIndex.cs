﻿using System.Linq;
using Raven.Client.Indexes;
using Snittlistan.Web.Areas.V2.ReadModels;

namespace Snittlistan.Web.Areas.V2.Indexes
{
    public class TeamOfWeekIndex : AbstractIndexCreationTask<TeamOfWeek>
    {
        public TeamOfWeekIndex()
        {
            Map = weeks => from week in weeks
                           select new
                           {
                               week.Season,
                               week.Turn
                           };
        }
    }
}