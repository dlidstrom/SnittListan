﻿namespace Snittlistan.Controllers
{
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.UI;
    using Raven.Client;
    using Snittlistan.Infrastructure.Indexes;

    /// <summary>
    /// Do not cache results from validation.
    /// </summary>
    [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    public class ValidationController : AbstractController
    {
        public ValidationController(IDocumentSession session)
            : base(session)
        {
        }

        public JsonResult IsBitsMatchIdAvailable(int bitsMatchId)
        {
            var id = Session.Query<Match_ByBitsMatchId.Result, Match_ByBitsMatchId>()
                .SingleOrDefault(m => m.BitsMatchId == bitsMatchId);

            // true is valid
            return Json(id == null, JsonRequestBehavior.AllowGet);
        }
    }
}