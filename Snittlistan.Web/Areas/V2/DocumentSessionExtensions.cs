﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Raven.Client;
using Snittlistan.Web.Areas.V2.Indexes;

namespace Snittlistan.Web.Areas.V2
{
    public static class DocumentSessionExtensions
    {
        public static List<SelectListItem> CreateRosterSelectList(this IDocumentSession session, int season, string rosterId = "")
        {
            return session.Query<RosterSearchTerms.Result, RosterSearchTerms>()
                .Where(x => x.Season == season)
                .Where(x => x.Preliminary == false)
                .Where(x => x.PlayerCount > 0)
                .Where(x => x.BitsMatchId != 0)
                .OrderBy(x => x.Date)
                .ProjectFromIndexFieldsInto<RosterSearchTerms.Result>()
                .ToList()
                .Where(x => x.MatchResultId == null || string.IsNullOrEmpty(rosterId) == false)
                .Select(
                    x => new SelectListItem
                    {
                        Text = $"{x.Turn}: {x.Team} - {x.Opponent} ({x.Location} {x.Date.ToShortTimeString()})",
                        Value = x.Id,
                        Selected = x.Id == rosterId
                    })
                .ToList();
        }
    }
}