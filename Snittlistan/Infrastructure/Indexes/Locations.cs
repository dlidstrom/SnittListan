﻿namespace Snittlistan.Infrastructure.Indexes
{
    using System.Linq;
    using Raven.Client.Indexes;
    using Snittlistan.Models;

    public class Locations : AbstractIndexCreationTask<Match, Locations.Result>
    {
        public Locations()
        {
            Map = matches => from match in matches
                             select new
                             {
                                 Location = match.Location
                             };

            Reduce = results => from result in results
                                group result by result.Location into g
                                select new
                                {
                                    Location = g.Key
                                };
        }

        public class Result
        {
            public string Location { get; set; }
        }
    }
}