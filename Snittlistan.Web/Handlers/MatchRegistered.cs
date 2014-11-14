﻿using Raven.Client;
using Snittlistan.Web.Areas.V2.Domain;
using Snittlistan.Web.Areas.V2.Domain.Match;
using Snittlistan.Web.Models;

namespace Snittlistan.Web.Handlers
{
    public class MatchRegistered : IHandle<MatchRegisteredEvent>
    {
        private readonly IDocumentSession documentSession;

        public MatchRegistered(IDocumentSession documentSession)
        {
            this.documentSession = documentSession;
        }

        public void Handle(MatchRegisteredEvent @event)
        {
            var roster = documentSession.Load<Roster>(@event.RosterId);
            Emails.MatchRegistered("Match registrerad", roster.Team, roster.Opponent, @event.Score, @event.OpponentScore);
        }
    }
}