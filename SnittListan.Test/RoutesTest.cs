﻿using System;
using System.Web.Routing;
using MvcContrib.TestHelper;
using SnittListan.Controllers;
using Xunit;
using System.Web.Mvc;

namespace SnittListan.Test
{
	public class RoutesTest : IDisposable
	{
		public RoutesTest()
		{
			new RouteConfigurator(RouteTable.Routes).Configure();
		}

		public void Dispose()
		{
			RouteTable.Routes.Clear();
		}

		[Fact]
		public void DefaultRoute()
		{
			"~/".ShouldMapTo<HomeController>(c => c.Index());
		}

		[Fact]
		public void LowerCaseRoutes()
		{
			"~/account/register".ShouldMapTo<AccountController>(c => c.Register());
		}

		[Fact]
		public void Shortcuts()
		{
			"~/register".ShouldMapTo<AccountController>(c => c.Register());
			"~/logon".ShouldMapTo<AccountController>(c => c.LogOn());
			"~/about".ShouldMapTo<HomeController>(c => c.About());
		}

		[Fact]
		public void Verify()
		{
			var verify = "~/verify".WithMethod(HttpVerbs.Post);
			var guid = Guid.NewGuid();
			verify.Values["activationKey"] = guid.ToString();
			verify.ShouldMapTo<AccountController>(c => c.Verify(guid));
		}
	}
}
