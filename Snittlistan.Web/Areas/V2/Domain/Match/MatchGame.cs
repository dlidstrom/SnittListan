﻿using System;
using System.Diagnostics;

namespace Snittlistan.Web.Areas.V2.Domain.Match
{
    [DebuggerDisplay("{Player} Pins={Pins} X={Strikes} Spares={Spares}")]
    public class MatchGame
    {
        public MatchGame(string player, int pins, int strikes, int spares)
        {
            if (player == null) throw new ArgumentNullException("player");
            this.Player = player;
            if (pins < 0 || pins > 300)
                throw new ArgumentException("Pins out of range", "pins");
            if (strikes < 0 || strikes > 12)
                throw new ArgumentException("Strikes out of range", "strikes");
            if (spares < 0 || spares > 10 || spares > Math.Max(0, 12 - strikes))
                throw new ArgumentException("Spares out of range", "spares");
            this.Pins = pins;
            this.Strikes = strikes;
            this.Spares = spares;
        }

        public string Player { get; private set; }

        public int Pins { get; private set; }

        public int Strikes { get; private set; }

        public int Spares { get; private set; }
    }
}