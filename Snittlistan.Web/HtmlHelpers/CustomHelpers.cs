﻿namespace Snittlistan.Web.HtmlHelpers
{
    using System.Web.Mvc;

    using Snittlistan.Web.Areas.V1.ViewModels.Match;

    public static class CustomHtmlHelpers
    {
        /// <summary>
        /// Returns HTML with an anchor link to BITS for the match.
        /// </summary>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <param name="model">Match view model.</param>
        /// <returns>The HTML markup with an anchor link to BITS.</returns>
        public static MvcHtmlString GenerateBitsLink(this HtmlHelper html, Match8x4ViewModel.MatchDetails model)
        {
            var builder = new TagBuilder("a");
            builder.MergeAttribute("href", string.Format("http://bits.swebowl.se/MatchFact.aspx?MatchId={0}", model.BitsMatchId));
            builder.SetInnerText("Matchfakta");

            return new MvcHtmlString(builder.ToString());
        }
    }
}