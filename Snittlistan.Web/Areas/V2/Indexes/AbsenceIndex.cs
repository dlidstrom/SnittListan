﻿#nullable enable

using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using Snittlistan.Web.Areas.V2.Domain;

namespace Snittlistan.Web.Areas.V2.Indexes;

public class AbsenceIndex : AbstractIndexCreationTask<Absence, AbsenceIndex.Result>
{
    public AbsenceIndex()
    {
        Map = absences => from absence in absences
                          select new
                          {
                              absence.Id,
                              absence.Player,
                              absence.From,
                              absence.To,
                              absence.Comment,
                              PlayerName = LoadDocument<Player>(absence.Player)
                                  .Name
                          };

        Store(x => x.Id, FieldStorage.Yes);
        Store(x => x.PlayerName, FieldStorage.Yes);
    }

    public class Result
    {
        public string? Id { get; set; }

        public string? Player { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public string? PlayerName { get; set; }

        public string? Comment { get; set; }
    }
}
