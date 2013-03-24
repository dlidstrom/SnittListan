﻿using System;
using System.Diagnostics;

namespace Snittlistan.Web.Areas.V2.Domain
{
    [DebuggerDisplay("{Game1.Player} {Game2.Player} {Score}")]
    public class MatchTable
    {
        public MatchTable(MatchGame game1, MatchGame game2, int score)
        {
            if (game1 == null) throw new ArgumentNullException("game1");
            if (game2 == null) throw new ArgumentNullException("game2");
            if (score != 0 && score != 1) throw new ArgumentOutOfRangeException("score", score, "Score out of range");
            if (game1.Player == game2.Player)
                throw new MatchException("Table must have different players");
            this.Score = score;
            this.Game1 = game1;
            this.Game2 = game2;
        }

        public MatchGame Game1 { get; private set; }

        public MatchGame Game2 { get; private set; }

        public int Score { get; private set; }
    }
}