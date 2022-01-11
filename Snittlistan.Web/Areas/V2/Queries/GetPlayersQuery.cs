using Raven.Client;
using Snittlistan.Web.Areas.V2.Domain;
using Snittlistan.Web.Infrastructure;

namespace Snittlistan.Web.Areas.V2.Queries;
public class GetPlayersQuery : IQuery<Player[]>
{
    private readonly List<string> playerIds;

    public GetPlayersQuery(Roster roster)
    {
        if (roster == null)
        {
            throw new ArgumentNullException(nameof(roster));
        }

        playerIds = roster.Players;
    }

    public Player[] Execute(IDocumentSession session)
    {
        Player[] result = session.Load<Player>(playerIds);
        return result;
    }
}
