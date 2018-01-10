﻿using System;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using NUnit.Framework;
using Snittlistan.Web.Areas.V1.Controllers;
using Snittlistan.Web.Areas.V1.Models;
using Snittlistan.Web.Areas.V1.ViewModels.Match;

namespace Snittlistan.Test.Controllers
{
    [TestFixture]
    public class MatchController_Details4x4 : DbTest
    {
        [Test]
        public void ShouldViewMatch()
        {
            // Arrange
            Session.Store(new Match4x4("P1", DateTime.Now, new Team4x4("Home1", 4), new Team4x4("Away1", 7)) { Id = 1 });
            Session.Store(new Match4x4("P2", DateTime.Now, new Team4x4("Home2", 5), new Team4x4("Away2", 8)) { Id = 2 });
            Session.Store(new Match4x4("P3", DateTime.Now, new Team4x4("Home3", 6), new Team4x4("Away3", 9)) { Id = 3 });
            var controller = new MatchController { DocumentSession = Session };

            // Act
            var view1 = controller.Details4x4(1) as ViewResult;
            Assert.NotNull(view1);
            Debug.Assert(view1 != null, "view1 != null");
            var model1 = view1.Model as Match4x4ViewModel;
            Debug.Assert(model1 != null, "model1 != null");
            Assert.That(model1.Match.Id, Is.EqualTo(1));

            var view2 = controller.Details4x4(2) as ViewResult;
            Assert.NotNull(view2);
            Debug.Assert(view2 != null, "view2 != null");
            var model2 = view2.Model as Match4x4ViewModel;
            Debug.Assert(model2 != null, "model2 != null");
            Assert.That(model2.Match.Id, Is.EqualTo(2));

            var view3 = controller.Details4x4(3) as ViewResult;
            Assert.NotNull(view3);
            Debug.Assert(view3 != null, "view3 != null");
            var model3 = view3.Model as Match4x4ViewModel;
            Debug.Assert(model3 != null, "model3 != null");
            Assert.That(model3.Match.Id, Is.EqualTo(3));
        }

        [Test]
        public void CannotViewNonExistingMatch()
        {
            var controller = new MatchController { DocumentSession = Session };
            try
            {
                controller.Details4x4(1);
                Assert.False(true, "Should throw");
            }
            catch (HttpException ex)
            {
                Assert.That(ex.GetHttpCode(), Is.EqualTo(404));
            }
        }
    }
}