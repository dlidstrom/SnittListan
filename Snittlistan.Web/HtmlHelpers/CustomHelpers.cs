﻿
using System.Web.Mvc;

namespace Snittlistan.Web.HtmlHelpers;
public static class CustomHtmlHelpers
{
    /// <summary>
    /// Returns link to BITS for match facts.
    /// </summary>
    /// <param name="html">The HTML helper instance that this method extends.</param>
    /// <param name="bitsMatchId">BITS match id.</param>
    /// <returns>The HTML markup with an anchor link to BITS.</returns>
    public static string GenerateBitsUrl(this HtmlHelper html, int bitsMatchId)
    {
        return GenerateBitsUrl(bitsMatchId);
    }

    public static string GenerateBitsUrl(int bitsMatchId)
    {
        return $"https://bits.swebowl.se/match-detail?matchid={bitsMatchId}";
    }
}
