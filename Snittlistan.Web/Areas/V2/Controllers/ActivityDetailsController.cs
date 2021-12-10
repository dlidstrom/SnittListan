﻿
using System.Web;
using System.Web.Mvc;
using Snittlistan.Web.Areas.V2.Domain;
using Snittlistan.Web.Areas.V2.ViewModels;
using Snittlistan.Web.Controllers;

#nullable enable

namespace Snittlistan.Web.Areas.V2.Controllers;
public class ActivityDetailsController : AbstractController
{
    public ActionResult Index(string id)
    {
        Activity activity = DocumentSession.Load<Activity>(id);
        if (activity == null)
        {
            throw new HttpException(404, "Not found");
        }

        ViewData["showComments"] = true;
        Player? player = null;
        if (activity.AuthorId != null)
        {
            player = DocumentSession.Load<Player>(activity.AuthorId);
        }

        ActivityViewModel activityViewModel =
            new(
                activity.Id!,
                activity.Title,
                activity.Date,
                activity.MessageHtml,
                player?.Name ?? string.Empty);
        return View(activityViewModel);
    }
}
