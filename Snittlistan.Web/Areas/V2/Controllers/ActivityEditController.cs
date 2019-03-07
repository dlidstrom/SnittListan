﻿namespace Snittlistan.Web.Areas.V2.Controllers
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using System.Web;
    using System.Web.Mvc;
    using Domain;
    using Helpers;
    using Raven.Abstractions;
    using ViewModels;
    using Web.Controllers;

    [Authorize(Roles = WebsiteRoles.Activity.Manage)]
    public class ActivityEditController : AbstractController
    {
        public ActionResult Create(int? season)
        {
            if (season.HasValue == false)
                season = DocumentSession.LatestSeasonOrDefault(SystemTime.UtcNow.Year);
            return View(ActivityEditViewModel.ForCreate(season.Value));
        }

        [HttpPost]
        public ActionResult Create(ActivityEditViewModel vm)
        {
            if (ModelState.IsValid == false)
            {
                return View(vm);
            }

            Debug.Assert(vm.Season != null, "vm.Season != null");
            Debug.Assert(vm.Date != null, "vm.Date != null");
            var activity =
                Activity.Create(
                    vm.Season.Value,
                    vm.Title,
                    vm.Date.Value,
                    vm.Message,
                    User.CustomIdentity.PlayerId);
            DocumentSession.Store(activity);

            return RedirectToAction("Index", "ActivityIndex");
        }

        public ActionResult Edit(string id)
        {
            var activity = DocumentSession.Load<Activity>(id);
            if (activity == null) throw new HttpException(404, "Activity not found");
            return View(ActivityEditViewModel.ForEdit(activity));
        }

        [HttpPost]
        public ActionResult Edit(string id, ActivityEditViewModel vm)
        {
            if (ModelState.IsValid == false) return View(vm);
            var activity = DocumentSession.Load<Activity>(id);
            if (activity == null) throw new HttpException(404, "Activity not found");
            Debug.Assert(vm.Season != null, "vm.Season != null");
            Debug.Assert(vm.Date != null, "vm.Date != null");
            activity.Update(
                vm.Season.Value,
                vm.Title,
                vm.Date.Value,
                vm.Message,
                User.CustomIdentity.PlayerId);
            return RedirectToAction("Index", "ActivityIndex");
        }

        public ActionResult Delete(string id)
        {
            var activity = DocumentSession.Load<Activity>(id);
            if (activity == null) throw new HttpException(404, "Activity not found");
            var player = DocumentSession.Load<Player>(activity.AuthorId);
            return View(new ActivityViewModel(activity.Id, activity.Title, activity.Date, activity.Message, player?.Name ?? string.Empty));
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteActivity(string id)
        {
            var activity = DocumentSession.Load<Activity>(id);
            if (activity != null) DocumentSession.Delete(activity);
            return RedirectToAction("Index", "ActivityIndex");
        }

        public class ActivityEditViewModel
        {
            [Required]
            [Display(Name = "Säsong")]
            public int? Season { get; set; }

            [Required]
            [MaxLength(80)]
            [DataType(DataType.Text)]
            [Display(Name = "Titel")]
            public string Title { get; set; }

            [Required]
            [DataType(DataType.DateTime)]
            [Display(Name = "Datum")]
            public DateTime? Date { get; set; }

            [Required]
            [MaxLength(1024)]
            [DataType(DataType.MultilineText)]
            [Display(Name = "Meddelande")]
            public string Message { get; set; }

            public static ActivityEditViewModel ForCreate(int season)
            {
                return new ActivityEditViewModel
                {
                    Season = season,
                    Date = SystemTime.UtcNow.Date.AddDays(1).AddHours(18)
                };
            }

            public static ActivityEditViewModel ForEdit(Activity activity)
            {
                return new ActivityEditViewModel
                {
                    Season = activity.Season,
                    Title = activity.Title,
                    Date = activity.Date,
                    Message = activity.Message
                };
            }
        }
    }
}