﻿#nullable enable

using System.Security.Principal;
using System.Web;
using Raven.Client;
using Snittlistan.Web.Models;

namespace Snittlistan.Web.HtmlHelpers;

public static class RequestExtensions
{
    public static bool IsAdmin(this HttpRequestBase request, IPrincipal principal)
    {
        IDocumentSession session = MvcApplication.Container.Resolve<IDocumentSession>();
        User admin = session.Load<User>(User.AdminId);
        return admin != null && request.IsAuthenticated && principal?.Identity.Name == admin.Email;
    }
}
