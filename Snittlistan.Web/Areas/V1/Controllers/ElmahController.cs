﻿namespace Snittlistan.Web.Areas.V1.Controllers
{
    using System.Web.Mvc;

    using Snittlistan.Web.Infrastructure.Results;

    [Authorize]
    public class ElmahController : Controller
    {
        public ActionResult Index(string type)
        {
            return new ElmahResult(type);
        }
    }
}