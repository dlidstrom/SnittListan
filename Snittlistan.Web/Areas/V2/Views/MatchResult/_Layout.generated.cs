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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Areas/V2/Views/MatchResult/_Layout.cshtml")]
    public partial class _Areas_V2_Views_MatchResult__Layout_cshtml : Snittlistan.Web.Infrastructure.BaseViewPage<dynamic>
    {
        public _Areas_V2_Views_MatchResult__Layout_cshtml()
        {
        }
        public override void Execute()
        {

            
            #line 1 "..\..\Areas\V2\Views\MatchResult\_Layout.cshtml"
  
    Layout = "~/Areas/V2/Views/Shared/_Layout.cshtml";


            
            #line default
            #line hidden
WriteLiteral("\r\n<div class=\"row\">\r\n    <div class=\"span12\">\r\n        <ul class=\"nav nav-pills\">" +
"\r\n            ");


            
            #line 8 "..\..\Areas\V2\Views\MatchResult\_Layout.cshtml"
       Write(Html.ActionMenuLink("Matcher", string.Empty, "Index", Url.Action("Index")));

            
            #line default
            #line hidden
WriteLiteral("\r\n            ");


            
            #line 9 "..\..\Areas\V2\Views\MatchResult\_Layout.cshtml"
       Write(Html.ActionMenuLink("Veckans lag", string.Empty, "Turns", Url.Action("Turns")));

            
            #line default
            #line hidden
WriteLiteral("\r\n            ");


            
            #line 10 "..\..\Areas\V2\Views\MatchResult\_Layout.cshtml"
       Write(Html.ActionMenuLink("Form", string.Empty, "Form", Url.Action("Form")));

            
            #line default
            #line hidden
WriteLiteral("\r\n            ");


            
            #line 11 "..\..\Areas\V2\Views\MatchResult\_Layout.cshtml"
       Write(Html.ActionMenuLink("Elitmärken", string.Empty, "EliteMedals", Url.Action("EliteMedals")));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </ul>\r\n    </div>\r\n</div>\r\n\r\n");


            
            #line 16 "..\..\Areas\V2\Views\MatchResult\_Layout.cshtml"
Write(RenderBody());

            
            #line default
            #line hidden
WriteLiteral("\r\n");


        }
    }
}
#pragma warning restore 1591