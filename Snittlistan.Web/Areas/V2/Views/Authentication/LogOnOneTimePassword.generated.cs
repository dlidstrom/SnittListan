﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Snittlistan.Web.Views
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    using Snittlistan.Web.Areas.V2;
    using Snittlistan.Web.Areas.V2.ViewModels;
    using Snittlistan.Web.HtmlHelpers;
    using Snittlistan.Web.Infrastructure;
    using Snittlistan.Web.Infrastructure.Database;
    using Snittlistan.Web.ViewModels;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Areas/V2/Views/Authentication/LogOnOneTimePassword.cshtml")]
    public partial class _Areas_V2_Views_Authentication_LogOnOneTimePassword_cshtml : Snittlistan.Web.Infrastructure.BaseViewPage<Snittlistan.Web.Areas.V2.Controllers.AuthenticationController.PasswordViewModel>
    {
        public _Areas_V2_Views_Authentication_LogOnOneTimePassword_cshtml()
        {
        }
        public override void Execute()
        {


            
            #line 2 "..\..\Areas\V2\Views\Authentication\LogOnOneTimePassword.cshtml"
  
    ViewBag.Title = "Logga in";


            
            #line default
            #line hidden
WriteLiteral("<div class=\"row\">\r\n    <div class=\"span8\">\r\n        <h3>");


            
            #line 7 "..\..\Areas\V2\Views\Authentication\LogOnOneTimePassword.cshtml"
       Write(ViewBag.Title);

            
            #line default
            #line hidden
WriteLiteral("</h3>\r\n");


            
            #line 8 "..\..\Areas\V2\Views\Authentication\LogOnOneTimePassword.cshtml"
         if (Model.ReuseToken)
        {

            
            #line default
            #line hidden
WriteLiteral("            <p>\r\n                Kolla i mailen där du ska ha fått dagens inloggn" +
"ingskod. Du kan använda samma kod igen för att logga in.\r\n            </p>\r\n");


            
            #line 13 "..\..\Areas\V2\Views\Authentication\LogOnOneTimePassword.cshtml"
            if (Model.ReusedTokenDate != null)
            {

            
            #line default
            #line hidden
WriteLiteral("                <p>Koden skickades ut ");


            
            #line 15 "..\..\Areas\V2\Views\Authentication\LogOnOneTimePassword.cshtml"
                                  Write(Model.ReusedTokenDate?.ToString("HH:mm"));

            
            #line default
            #line hidden
WriteLiteral(" idag.</p>\r\n");


            
            #line 16 "..\..\Areas\V2\Views\Authentication\LogOnOneTimePassword.cshtml"
            }
        }
        else
        {

            
            #line default
            #line hidden
WriteLiteral("            <p>Dagens inloggningskod har skickats till din e-postadress. Du kan a" +
"nvända samma kod hela dagen så passa på att logga in i alla dina enheter på en g" +
"ång.</p>\r\n");


            
            #line 21 "..\..\Areas\V2\Views\Authentication\LogOnOneTimePassword.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("\r\n        <p>\r\n            <strong style=\"background-color: yellow; padding: 5px;" +
" border: 1px dashed red;\">\r\n                Om du har problem att logga in, öppn" +
"a <a href=\"//");


            
            #line 25 "..\..\Areas\V2\Views\Authentication\LogOnOneTimePassword.cshtml"
                                                            Write(Model.Hostname);

            
            #line default
            #line hidden
WriteLiteral("\">");


            
            #line 25 "..\..\Areas\V2\Views\Authentication\LogOnOneTimePassword.cshtml"
                                                                             Write(Model.Hostname);

            
            #line default
            #line hidden
WriteLiteral("</a> i en ny flik och prova att logga in där.\r\n            </strong>\r\n        </p" +
">\r\n\r\n");


            
            #line 29 "..\..\Areas\V2\Views\Authentication\LogOnOneTimePassword.cshtml"
         using (Html.BeginForm("LogOnOneTimePassword", "Authentication", FormMethod.Post, new { @class = "form-horizontal" }))
        {
            
            
            #line default
            #line hidden
            
            #line 31 "..\..\Areas\V2\Views\Authentication\LogOnOneTimePassword.cshtml"
       Write(Html.HiddenFor(x => x.OneTimeKey));

            
            #line default
            #line hidden
            
            #line 31 "..\..\Areas\V2\Views\Authentication\LogOnOneTimePassword.cshtml"
                                              
            Html.RenderPartial("_PasswordEditor");
        }

            
            #line default
            #line hidden
WriteLiteral("    </div>\r\n</div>\r\n");


        }
    }
}
#pragma warning restore 1591
