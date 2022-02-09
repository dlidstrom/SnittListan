﻿
using EventStoreLite;

#nullable enable

namespace Snittlistan.Web.Areas.V2.Domain.Match.Events;
public class ClearMedals : Event
{
    public ClearMedals(int bitsMatchId, string rosterId)
    {
        BitsMatchId = bitsMatchId;
        RosterId = rosterId;
    }

    public int BitsMatchId { get; private set; }

    public string RosterId { get; private set; }
}
