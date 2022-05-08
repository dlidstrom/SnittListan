﻿#nullable enable

using System.Web.Mvc;
using Raven.Abstractions;
using Snittlistan.Web.Areas.V2.Domain;
using Snittlistan.Web.Areas.V2.Indexes;
using Snittlistan.Web.Areas.V2.ViewModels;
using Snittlistan.Web.Infrastructure.Indexes;
using Snittlistan.Web.Models;

namespace Snittlistan.Web;

public static class Extensions
{
    public static SelectListItem[] CreateRosterSelectList(
        this Raven.Client.IDocumentSession session,
        int season,
        string rosterId = "",
        Func<Roster, bool>? pred = null)
    {
        return GetRosterSelectList(session, season, rosterId, false, pred ?? (x => true));
    }

    public static RosterViewModel LoadRosterViewModel(
        this Raven.Client.IDocumentSession session,
        Roster roster)
    {
        HashSet<string> accepted = new(roster.AcceptedPlayers);
        List<RosterViewModel.PlayerItem> players = new();
        foreach (Player player in roster.Players.Where(p => p != null).Select(session.Load<Player>))
        {
            players.Add(new RosterViewModel.PlayerItem(player.Id, player.Name, accepted.Contains(player.Id)));
        }

        RosterViewModel.PlayerItem? teamLeaderTuple = null;
        if (roster.TeamLeader != null)
        {
            Player teamLeader = session.Load<Player>(roster.TeamLeader);
            teamLeaderTuple = new RosterViewModel.PlayerItem(
                teamLeader.Id,
                teamLeader.Name,
                accepted.Contains(teamLeader.Id));
        }

        RosterViewModel vm = new(roster, teamLeaderTuple, players);
        return vm;
    }

    public static SelectListItem[] CreatePlayerSelectList(
        this Raven.Client.IDocumentSession documentSession,
        string player = "",
        Func<Player[]>? getPlayers = null,
        Func<Player, string>? textFormatter = null)
    {
        List<SelectListItem> playerList = new()
        {
            new SelectListItem
            {
                Text = "Välj medlem",
                Value = string.Empty
            }
        };
        Player[] query =
            getPlayers != null
                ? getPlayers.Invoke()
                : documentSession.Query<Player, PlayerSearch>()
                                 .Where(x => x.PlayerStatus == Player.Status.Active)
                                 .ToArray();

        SelectListItem[] players = query.OrderBy(x => x.Name)
                           .Select(x => new SelectListItem
                           {
                               Text = textFormatter != null ? textFormatter.Invoke(x) : x.Name,
                               Value = x.Id,
                               Selected = x.Id == player
                           })
                           .ToArray();
        playerList.AddRange(players);
        return playerList.ToArray();
    }

    public static TResult LoadEx<TResult>(this Raven.Client.IDocumentSession session, string key)
    {
        return session.Load<TResult>(key) ?? throw new Exception($"No entity with key '{key}' found");
    }

    public static User FindUserByEmail(this Raven.Client.IDocumentSession sess, string email)
    {
        return sess.Query<User, User_ByEmail>()
                   .FirstOrDefault(u => u.Email == email);
    }

    private static SelectListItem[] GetRosterSelectList(
        Raven.Client.IDocumentSession session,
        int season,
        string rosterId,
        bool bits,
        Func<Roster, bool> pred)
    {
        IQueryable<Roster> query = session.Query<Roster, RosterSearchTerms>()
                           .Where(x => x.Season == season)
                           .Where(x => x.Date < SystemTime.UtcNow)
                           .Where(x => x.Preliminary == false);
        if (bits)
        {
            query = query.Where(x => x.BitsMatchId != 0);
        }
        else
        {
            query = query.Where(x => x.BitsMatchId == 0);
        }

        SelectListItem[] rosterSelectList = query.OrderBy(x => x.Date)
                                    .ToList()
                                    .Where(x => x.MatchResultId == null || string.IsNullOrEmpty(rosterId) == false)
                                    .Where(pred)
                                    .Select(
                                        x => new SelectListItem
                                        {
                                            Text = $"{x.Turn}: {x.Team} - {x.Opponent} ({x.Location} {x.Date.ToShortTimeString()})",
                                            Value = x.Id,
                                            Selected = x.Id == rosterId
                                        })
                                    .ToArray();
        return rosterSelectList;
    }
}
